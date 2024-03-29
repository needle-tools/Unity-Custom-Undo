﻿using UnityEditor;
using UnityEngine;

namespace Needle
{
	[InitializeOnLoad]
	internal class UnityCommandMock : ScriptableObject
	{
		static UnityCommandMock()
		{
			CustomUndo.requestEditorMock += RegisterCustomCommand;
		}
		
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
			// Undo.IncrementCurrentGroup();
		}
		
		public uint value;
	}
}