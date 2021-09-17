using UnityEditor;

namespace Needle
{
	public static class CustomUndo
	{
		private static CommandQueue Commands { get; } = new CommandQueue();

		public static void Register(ICommand command)
		{
			if (Commands.Enqueue(command))
			{
				UnityCommandMock.RegisterCustomCommand(command.Name);
			}
		}

		internal static void OnUndo(string _)
		{
			if (!Commands.Undo())
			{
				// if a command for some reason can not run perform another Unity undo?
				// Undo.PerformUndo();
			}
		}

		internal static void OnRedo(string _)
		{
			if (!Commands.Redo())
			{
				// if a command for some reason can not run perform another Unity redo?
				// Undo.PerformRedo();
			}
		}
	}
}