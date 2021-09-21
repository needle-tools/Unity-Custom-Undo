using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Needle
{
	internal static class UnityUndoTracker
	{
		internal static IReadOnlyList<string> UndoRecords => undoRecords;
		internal static IReadOnlyList<string> RedoRecords => redoRecords;


		private static readonly List<string> undoRecords = new List<string>();
		private static readonly List<string> redoRecords = new List<string>();
		private static int lastUndoRecordsCount, lastRedoRecordsCount;

		internal static event Action<string> UnityUndoPerformed, UnityRedoPerformed;
		
		[InitializeOnLoadMethod]
		private static void Init()
		{
			Undo.undoRedoPerformed -= OnUndoRedo;
			Undo.undoRedoPerformed += OnUndoRedo;
			UpdateLists();
			UpdateCounts();
		}

		private static void OnUndoRedo()
		{
			UpdateLists();
			if (undoRecords.Count > lastUndoRecordsCount)
			{
				// was redo
				UnityRedoPerformed?.Invoke(undoRecords.LastOrDefault());
			}
			else if (redoRecords.Count > lastRedoRecordsCount)
			{
				// was undo
				UnityUndoPerformed?.Invoke(redoRecords.LastOrDefault());
			}
			UpdateCounts();
		}

		private static MethodBase getRecordsMethod;
		private static object[] parameters;

		internal static void OnDidPerformMockCommand()
		{
			UpdateLists();
			UpdateCounts();
		}

		private static void UpdateLists()
		{
			undoRecords.Clear();
			redoRecords.Clear();
			if (getRecordsMethod == null)
			{
				getRecordsMethod = typeof(UnityEditor.Undo).GetMethod("GetRecords", BindingFlags.NonPublic | BindingFlags.Static);
				parameters = new object[] { undoRecords, redoRecords };
			}
			getRecordsMethod?.Invoke(null, parameters);
		}
		
		private static void UpdateCounts()
		{
			lastUndoRecordsCount = undoRecords.Count;
			lastRedoRecordsCount = redoRecords.Count;
		}
	}
}