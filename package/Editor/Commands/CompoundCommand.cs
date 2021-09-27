using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Needle
{
	public class CompoundCommand : Command
	{
		private readonly IList<ICommand> _commands;
		
		public CompoundCommand(IEnumerable<ICommand> col)
		{
			this._commands = col.ToArray();
		}
		
		public CompoundCommand(params ICommand[] commands)
		{
			this._commands = commands;
		}
		
		protected override void OnRedo()
		{
			foreach (var cmd in _commands)
			{
				cmd.PerformRedo();
			}
		}

		protected override void OnUndo()
		{
			for (var index = _commands.Count - 1; index >= 0; index--)
			{
				var cmd = _commands[index];
				cmd.PerformUndo();
			}
		}
	}
}