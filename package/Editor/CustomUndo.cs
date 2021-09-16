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
			Commands.Undo();
		}

		internal static void OnRedo(string _)
		{
			Commands.Redo();
		}
	}
}