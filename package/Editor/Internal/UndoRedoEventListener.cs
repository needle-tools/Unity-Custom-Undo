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
				Debug.Log("<b>Custom redo</b>: " + obj);
				CustomUndo.OnRedo(obj);
			}
			else Debug.Log("<b>Unity redo</b>: " + obj);
		}

		private static void OnUndo(string obj)
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				Debug.Log("<b>Custom undo</b>: " + obj);
				CustomUndo.OnUndo(obj);
			}
			else Debug.Log("<b>Unity undo</b>: " + obj);
		}
	}
}
