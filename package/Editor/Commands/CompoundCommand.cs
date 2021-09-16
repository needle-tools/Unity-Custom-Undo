using System.Threading.Tasks;

namespace Needle
{
	public class CompoundCommand : Command
	{
		private readonly ICommand[] _commands;
		
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
			for (var index = _commands.Length - 1; index >= 0; index--)
			{
				var cmd = _commands[index];
				cmd.PerformUndo();
			}
		}
	}
}