using UnityEditor;
using UnityEngine;

namespace Needle.UndoEverything
{
	[InitializeOnLoad]
	internal static class UndoListener
	{
		static UndoListener()
		{
			UnityUndoHelper.UnityUndoPerformed += OnUndo;
			UnityUndoHelper.UnityRedoPerformed += OnRedo;
		}

		private static void OnRedo(string obj)
		{
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				UndoEverything.OnRedo(obj);
			}
			else Debug.Log("Unity redo: " + obj);
		}

		private static void OnUndo(string obj)
		{
			if (obj.EndsWith(UnityCommandMock.CommandMarker))
			{
				UndoEverything.OnUndo(obj);
			}
			else Debug.Log("Unity undo: " + obj);
		}
	}
}