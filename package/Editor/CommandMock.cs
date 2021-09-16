using UnityEditor;
using UnityEngine;

namespace Needle.UndoEverything
{
	internal class UnityCommandMock : ScriptableObject
	{
		// could be invisible utf8 maybe
		internal const string CommandMarker = "                                           _Needle_Command";
		
		private static UnityCommandMock _instance;
		public static void RegisterCustomCommand(string name)
		{
			if (!_instance)
			{
				_instance = CreateInstance<UnityCommandMock>();
				_instance.hideFlags = HideFlags.HideAndDontSave; 
			}

			Undo.RegisterCompleteObjectUndo(_instance, name + CommandMarker);
			_instance.value += 1;
			UnityUndoHelper.OnDidPerformMockCommand();
		}
		
		public uint value;
	}
}