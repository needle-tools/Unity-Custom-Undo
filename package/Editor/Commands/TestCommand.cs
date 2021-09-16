using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Needle.UndoEverything
{
	public class TestCommand : Command
	{
		public DateTime time = DateTime.Now;

		protected override void OnRedo()
		{
			Debug.Log("Redo " + time);
		}

		protected override void OnUndo()
		{
			Debug.Log("Undo " + time);
			DragAndDrop.
		}
	}
}