using System.Collections;
using System.Collections.Generic;

namespace Needle
{
	public static class Extensions
	{
		public static CompoundCommand ToCompound(this IEnumerable<ICommand> commands, string name, bool done = false)
		{
			return new CompoundCommand(commands) { Name = name, IsDone = done};
		}
	}
}