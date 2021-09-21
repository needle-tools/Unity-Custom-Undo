namespace Needle
{
	public static class UndoHelper
	{
		public static void Undo(int count)
		{
			if (UnityUndoTracker.UndoRecords == null) return;
			for (var i = 0; i < count; i++)
			{
				if (UnityUndoTracker.UndoRecords.Count <= 0) return;
				UnityEditor.Undo.PerformUndo();
			}
		}

		public static void Redo(int count)
		{
			if (UnityUndoTracker.RedoRecords == null) return;
			for (var i = 0; i < count; i++)
			{
				if (UnityUndoTracker.RedoRecords.Count <= 0) return;
				UnityEditor.Undo.PerformRedo();
			}
		}
	}
}