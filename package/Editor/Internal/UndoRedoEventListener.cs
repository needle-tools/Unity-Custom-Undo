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
			UnityUndoTracker.UnityRedoPerformed -= OnRedo;
			
			UnityUndoTracker.UnityUndoPerformed += OnUndo;
			UnityUndoTracker.UnityRedoPerformed += OnRedo;
		}

		private static void OnRedo(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				EditorLog.Log("<b>Custom redo</b>: " + obj);
				CustomUndo.OnRedo(obj);
			}
			else EditorLog.Log("<b>Unity redo</b>: " + obj);
		}

		private static void OnUndo(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				EditorLog.Log("<b>Custom undo</b>: " + obj);
				CustomUndo.OnUndo(obj);
			}
			else EditorLog.Log("<b>Unity undo</b>: " + obj);
		}
	}
}
