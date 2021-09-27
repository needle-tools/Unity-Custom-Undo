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


		private static readonly List<string> undoRecords = new List<string>(), redoRecords = new List<string>();
		private static readonly List<string> previousUndo = new List<string>(), previousRedo = new List<string>();
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


			// we need to have another list because one undo can undo multiple operations (e.g. selection + value change) 
			// so to capture all potential custom undo actions
			previousUndo.Clear();
			previousUndo.AddRange(undoRecords);
			previousRedo.Clear();
			previousRedo.AddRange(redoRecords);

			UpdateLists();
			if (undoRecords.Count > previousUndo.Count)
			{
				// was redo
				wasRedo = true;
			}
			else if (redoRecords.Count > previousRedo.Count)
			{
				// was undo
				wasUndo = true;
			}

			UpdateCounts();

			if (wasRedo)
			{
				var diff = undoRecords.Count - previousUndo.Count;
				for (var i = 0; i < diff; i++)
				{
					var index = previousUndo.Count + i;
					var rec = undoRecords[index];
					UnityRedoPerformed?.Invoke(rec);
				}
			}
			if (wasUndo)
			{
				var diff = redoRecords.Count - previousRedo.Count;
				for (var i = 0; i < diff; i++)
				{
					var index = previousRedo.Count + i;
					var rec = redoRecords[index];
					UnityUndoPerformed?.Invoke(rec);
				}
			}
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