using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Needle
{
	public class UndoStackWindow : EditorWindow
	{
		[MenuItem("Needle/UndoRedo/Stack")]
		private new static void Show()
		{
			if (HasOpenInstances<UndoStackWindow>())
				FocusWindowIfItsOpen<UndoStackWindow>();
			else
			{
				var window = CreateInstance<UndoStackWindow>();
				((EditorWindow)window).Show();
			}
		}

		private void OnEnable()
		{
			titleContent = new GUIContent("Undo Stack");
			currentStyle = null;
			Undo.undoRedoPerformed += Repaint;
			CustomUndo.DidInjectCustomCommand += () =>
			{
				UnityUndoTracker.Refresh();
				Repaint();
			};
			EditorApplication.hierarchyChanged += Refresh;
			Selection.selectionChanged += Refresh;
			Undo.willFlushUndoRecord += Refresh;
			EditorApplication.projectChanged += Refresh;
		}

		private Vector2 scroll;
		private GUIStyle currentStyle;
		private GUILayoutOption[] buttonOptions;

		private void OnGUI()
		{
			if (currentStyle == null)
			{
				currentStyle = new GUIStyle(EditorStyles.boldLabel);
				currentStyle.alignment = TextAnchor.MiddleCenter;
			}

			EditorGUILayout.BeginVertical();
			scroll = EditorGUILayout.BeginScrollView(scroll);
			
			for (var index = UnityUndoTracker.RedoRecords.Count - 1; index >= 0; index--)
			{
				var str = UnityUndoTracker.RedoRecords[index];
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button(
						new GUIContent(str.Replace(UnityCommandMock.CommandMarker, string.Empty), "Redo"),
						buttonOptions))
					{
						UndoHelper.Redo(index+1);
						return;
					}
				}
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("You are here", currentStyle);
			}

			// EditorGUILayout.LabelField("Undo", EditorStyles.boldLabel);
			for (var index = UnityUndoTracker.UndoRecords.Count - 1; index >= 0; index--)
			{
				var str = UnityUndoTracker.UndoRecords[index];
				if (GUILayout.Button(
					new GUIContent(str.Replace(UnityCommandMock.CommandMarker, string.Empty), "Undo"),
					buttonOptions))
				{
					UndoHelper.Undo(UnityUndoTracker.UndoRecords.Count - index);
					return;
				}
			}

			foreach (var cmd in CustomUndo._customCommandsQueue.Commands)
			{
				EditorGUILayout.LabelField(cmd.ToString());
			}
			
			EditorGUILayout.EndScrollView();

			CustomUndo.LogToConsole = EditorGUILayout.ToggleLeft("DebugLog", CustomUndo.LogToConsole);
			
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Unity Undo", GUILayout.Height(50)))
			{
				Undo.PerformUndo();
				return;
			}
			if (GUILayout.Button("Unity Redo", GUILayout.Height(50)))
			{
				Undo.PerformRedo();
				return;
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Custom Undo", GUILayout.Height(20)))
			{
				CustomUndo.OnUndo(string.Empty);
				return;
			}
			if (GUILayout.Button("Custom Redo", GUILayout.Height(20)))
			{
				CustomUndo.OnRedo(string.Empty);
				return;
			}
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Refresh", GUILayout.Height(50)))
			{
				UnityUndoTracker.Refresh();
			}
			


			GUILayout.Space(10);
			if (GUILayout.Button("Clear Stack", GUILayout.Height(25)))
			{
				Undo.ClearAll();
				CustomUndo.Clear();
				UnityUndoTracker.Refresh();
				Refresh();
			}
			GUILayout.Space(6);
			EditorGUILayout.EndVertical();
		}

		private void Refresh()
		{
			Repaint();
			InternalEditorUtility.RepaintAllViews();
		}
	}
}