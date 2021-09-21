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
		
		public static void LogWarning(object msg, object context = null)
		{
			if (CustomUndo.LogToConsole)
				Debug.LogWarning(msg, context as Object);
		}
		
		public static void LogError(object msg, object context = null)
		{
			if (CustomUndo.LogToConsole)
				Debug.LogError(msg, context as Object);
		}
	}
}