using UnityEditor;
using UnityEngine;

namespace Needle
{
	public class SetDirtyCommand : Command
	{
		
#if UNITY_EDITOR
		private readonly Object obj;
		public SetDirtyCommand(Object obj)
		{
			this.obj = obj;
		}
#endif

		protected override void OnRedo()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(obj);
#endif
		}

		protected override void OnUndo()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(obj);
#endif
		}
	}
}