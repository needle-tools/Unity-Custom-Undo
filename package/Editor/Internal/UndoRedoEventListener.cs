using UnityEditor;
using UnityEngine;

namespace Needle
{
	[InitializeOnLoad]
	internal static class UndoRedoEventListener
	{
		static UndoRedoEventListener()
		{
			UnityUndoTracker.UnityUndoPerformed += OnUndo;
			UnityUndoTracker.UnityRedoPerformed += OnRedo;
		}

		private static void OnRedo(string obj)
		{
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				CustomUndo.OnRedo(obj);
			}
			else Debug.Log("Unity redo: " + obj);
		}

		private static void OnUndo(string obj)
		{
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				CustomUndo.OnUndo(obj);
			}
			else Debug.Log("Unity undo: " + obj);
		}
	}
}
