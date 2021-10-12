using UnityEditor;

namespace Needle
{
	public class CustomUndoSettings
	{
		private static bool logToConsole;
		public static bool LogToConsole
		{
			#if UNITY_EDITOR
			get => SessionState.GetBool("CustomUndoLog", false);
			set => SessionState.SetBool("CustomUndoLog", value);
			#else
			get => logToConsole;
			set => logToConsole = value;
			#endif
		}

	}
}