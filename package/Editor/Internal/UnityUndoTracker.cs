using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
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
			bool wasRedo = false, wasUndo = false;
			var undo = undoRecords.LastOrDefault();
			var redo = redoRecords.LastOrDefault();
			UpdateLists();
			if (undoRecords.Count > lastUndoRecordsCount)
			{
				// was redo
				wasRedo = true;
			}
			else if (redoRecords.Count > lastRedoRecordsCount)
			{
				// was undo
				wasUndo = true;
			}

			UpdateCounts();

			if (wasRedo)
				UnityRedoPerformed?.Invoke(undoRecords.LastOrDefault());
			if (wasUndo)
				UnityUndoPerformed?.Invoke(redoRecords.LastOrDefault());
		}

		private static MethodBase getRecordsMethod;
		private static object[] parameters;
		private static bool isRefreshing;

		internal static void Refresh()
		{
			if (isRefreshing) return;
			isRefreshing = true;
			try
			{
				UpdateLists();
				UpdateCounts();
			}
			finally
			{
				isRefreshing = false;
			}
		}

		private static void UpdateLists()
		{
			undoRecords.Clear();
			redoRecords.Clear();
			if (getRecordsMethod == null)
			{
				getRecordsMethod = typeof(UnityEditor.Undo).GetMethod("GetRecords", BindingFlags.NonPublic | BindingFlags.Static, null, CallingConventions.Any, new []{typeof(List<string>), typeof(List<string>)}, null);
				parameters = new object[] { undoRecords, redoRecords };
			}
			Undo.FlushUndoRecordObjects();
			getRecordsMethod?.Invoke(null, parameters);
			CleanRecords(undoRecords);
			CleanRecords(redoRecords);//, undoRecords[undoRecords.Count-1] == selectionChangedRecord);
		}
		
		private static readonly string[] selectionChangedRecord = {"Selection Change", "Clear Selection"};

		private static void UpdateCounts()
		{
			lastUndoRecordsCount = undoRecords.Count;
			lastRedoRecordsCount = redoRecords.Count;
		}

		private static void CleanRecords(IList<string> list, bool allowSkipFirst = false)
		{
			var wasSelectionChanged = false;
			for (var i = list.Count - 1; i >= 0; i--)
			{
				if (!selectionChangedRecord.Contains(list[i]))
				{
					wasSelectionChanged = false;
				}
				else if (selectionChangedRecord.Contains(list[i]))
				{
					if (wasSelectionChanged || (i == list.Count-1 && allowSkipFirst)) list.RemoveAt(i);
					wasSelectionChanged = true;
				}
			}
		}
	}
}