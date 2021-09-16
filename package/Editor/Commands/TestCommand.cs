using UnityEngine;

namespace Needle.UndoEverything
{
	public class TestCommand : Command
	{
		protected override void OnRedo()
		{
			Debug.Log("Do " + this);
		}

		protected override void OnUndo()
		{
			Debug.Log("Undo " + this);
		}
	}
}