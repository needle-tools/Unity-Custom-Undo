using System;
using System.Diagnostics.SymbolStore;
using UnityEditor;
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
			Undo.undoRedoPerformed += Repaint;
			CustomUndo.DidInjectCustomCommand += Repaint;
			currentStyle = null;
			// buttonOptions = new[]
			// {
			//	GUILayout.MaxWidth(140)
			// };
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

			scroll = EditorGUILayout.BeginScrollView(scroll);

			
			// EditorGUILayout.LabelField("Redo", EditorStyles.boldLabel);
			for (var index = 0; index < UnityUndoTracker.RedoRecords.Count; index++)
			{
				var str = UnityUndoTracker.RedoRecords[index];
				using (new EditorGUILayout.HorizontalScope())
				{
					if (GUILayout.Button(str.Replace(UnityCommandMock.CommandMarker, string.Empty), buttonOptions))
					{
						UndoHelper.Redo(UnityUndoTracker.RedoRecords.Count - (index));
						break;
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
				if (GUILayout.Button(str.Replace(UnityCommandMock.CommandMarker, string.Empty), buttonOptions))
				{
					UndoHelper.Undo(UnityUndoTracker.UndoRecords.Count - index);
					break;
				}
			}

			EditorGUILayout.EndScrollView();
		}
	}
}