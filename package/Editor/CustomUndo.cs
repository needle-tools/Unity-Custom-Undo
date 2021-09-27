using System;
using UnityEditor;

namespace Needle
{
	public static class CustomUndo
	{
		public static bool LogToConsole
		{
			get => SessionState.GetBool("CustomUndoLog", false);
			set => SessionState.SetBool("CustomUndoLog", value);
		}

		public static event Action DidInjectCustomCommand;

		public static void Clear() => _customCommandsQueue.Clear();

		public static void Register(ICommand command)
		{
			if (_customCommandsQueue.Enqueue(command))
			{
				UnityCommandMock.RegisterCustomCommand(command.Name);
				DidInjectCustomCommand?.Invoke();
			}
		}

		internal static CommandQueue _customCommandsQueue { get; } = new CommandQueue();

		internal static void OnUndo(string _)
		{
			if (!_customCommandsQueue.Undo())
			{
				// if a command for some reason can not run perform another Unity undo?
				// Undo.PerformUndo();
			}
		}

		internal static void OnRedo(string _)
		{
			if (!_customCommandsQueue.Redo())
			{
				// if a command for some reason can not run perform another Unity redo?
				// Undo.PerformRedo();
			}
		}



		internal static void OnRemoveCommand(string _)
		{
			_customCommandsQueue.RemoveCommands();
		}
	}
}