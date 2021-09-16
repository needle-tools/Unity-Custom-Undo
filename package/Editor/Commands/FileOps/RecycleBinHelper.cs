using System.IO;
using UnityEditor;

namespace Needle
{
	internal static class RecycleBinHelper
	{
		// public const string Path = @"C:\$Recycle.Bin";
		
		private static string tempBin => Path.GetTempPath() + "/tempRecycleBin";

		[InitializeOnLoadMethod]
		private static void Init()
		{
			// TODO: clear temp bin
		}
		
		public static bool MoveToRecycleBin(string filePath, out string binPath)
		{
			var fileInfo = new FileInfo(filePath);
			var tempDir = Path.GetTempPath();
			var targetDir = fileInfo.Directory != null ? tempBin + "/" + fileInfo.Directory.Name : tempBin;
			if (!Directory.Exists(targetDir))
				Directory.CreateDirectory(targetDir);
			binPath = targetDir + "/" + fileInfo.Name;
			if (File.Exists(binPath)) File.Delete(binPath);
			File.Move(filePath, binPath);
			return true;
		}
	}
}