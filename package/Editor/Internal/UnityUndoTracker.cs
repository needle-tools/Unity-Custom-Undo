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
				getRecordsMethod = typeof(UnityEditor.Undo).GetMethod("GetRecords", BindingFlags.NonPublic | BindingFlags.Static);
				parameters = new object[] { undoRecords, redoRecords };
			}
			Undo.FlushUndoRecordObjects();
			getRecordsMethod?.Invoke(null, parameters);
		}

		private static void UpdateCounts()
		{
			lastUndoRecordsCount = undoRecords.Count;
			lastRedoRecordsCount = redoRecords.Count;
		}
	}
}