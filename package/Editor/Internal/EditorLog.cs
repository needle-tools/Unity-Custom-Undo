using UnityEngine;

namespace Needle
{
	internal static class EditorLog
	{
		public static void Log(object msg, object context = null)
		{
			if (CustomUndo.LogToConsole)
				Debug.Log(msg, context as Object);
		}
	}
}