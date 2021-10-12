using UnityEditor;
using UnityEngine;

namespace Needle
{
	internal static class UndoRedoEventListener
	{
		[InitializeOnLoadMethod]
		private static void Init()
		{
			UnityUndoTracker.UnityUndoPerformed -= OnUndo;
			UnityUndoTracker.UnityUndoPerformed += OnUndo;
			
			UnityUndoTracker.UnityRedoPerformed -= OnRedo;
			UnityUndoTracker.UnityRedoPerformed += OnRedo;

			UnityUndoTracker.RemoveCommands -= OnRemoveCommands;
			UnityUndoTracker.RemoveCommands += OnRemoveCommands;
		}

		private static void OnRemoveCommands(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				CustomUndo.OnRemoveCommand(obj);
			}
		}

		private static void OnRedo(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				UndoLog.Log("<b>Custom redo</b>: " + obj);
				CustomUndo.OnRedo(obj);
			}
			else UndoLog.Log("<b>Unity redo</b>: " + obj);
		}

		private static void OnUndo(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				UndoLog.Log("<b>Custom undo</b>: " + obj);
				CustomUndo.OnUndo(obj);
			}
			else UndoLog.Log("<b>Unity undo</b>: " + obj);
		}
	}
}
