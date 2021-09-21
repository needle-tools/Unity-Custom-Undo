using System;
using System.Diagnostics.SymbolStore;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

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
			CustomUndo.DidInjectCustomCommand += Repaint;
			EditorApplication.hierarchyChanged += Refresh;
			Selection.selectionChanged += Refresh;
			Undo.willFlushUndoRecord += Refresh;
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

			// EditorGUILayout.LabelField("Redo", EditorStyles.boldLabel);
			for (var index = 0; index < UnityUndoTracker.RedoRecords.Count; index++)
			{
				var str = UnityUndoTracker.RedoRecords[index];
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button(
						new GUIContent(str.Replace(UnityCommandMock.CommandMarker, string.Empty), "Redo"),
						buttonOptions))
					{
						UndoHelper.Redo(UnityUndoTracker.RedoRecords.Count - (index));
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

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Custom Undo", GUILayout.Height(50)))
			{
				CustomUndo.OnUndo(string.Empty);
			}
			if (GUILayout.Button("Custom Redo", GUILayout.Height(50)))
			{
				CustomUndo.OnRedo(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			if (GUILayout.Button("Refresh", GUILayout.Height(50)))
			{
				Refresh();
			}
			if (GUILayout.Button("Clear Stack"))
			{
				Undo.ClearAll();
				CustomUndo.Clear();
				Refresh();
			}
			EditorGUILayout.EndVertical();
		}

		private void Refresh()
		{
			UnityUndoTracker.Refresh();
			Repaint();
			InternalEditorUtility.RepaintAllViews();
		}
	}
}