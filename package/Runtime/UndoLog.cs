using UnityEngine;

namespace Needle
{
	public static class UndoLog
	{
		public static void Log(object msg, object context = null)
		{
			if (CustomUndoSettings.LogToConsole)
				Debug.Log(msg, context as Object);
		}
		
		public static void LogWarning(object msg, object context = null)
		{
			if (CustomUndoSettings.LogToConsole)
				Debug.LogWarning(msg, context as Object);
		}
		
		public static void LogError(object msg, object context = null)
		{
			if (CustomUndoSettings.LogToConsole)
				Debug.LogError(msg, context as Object);
		}
	}
}