using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Needle
{
	public class DragDropUndo : Command
	{
		// public static DragDropUndo Create(int dragId)
		// {
		// 	
		// }

		private readonly DropEventArgs args;
		[CanBeNull] private List<MoveInfo> movedFiles;

		public DragDropUndo(DropEventArgs args) : base(true)
		{
			this.args = args;
			EditorLog.Log(args);
		}

		protected override void OnRedo()
		{
			if (movedFiles == null || movedFiles.Count <= 0) return;
			var requireRefresh = false;
			switch (args.dropResult)
			{
				case DragAndDropVisualMode.Copy:
					for (var index = movedFiles.Count - 1; index >= 0; index--)
					{
						var moved = movedFiles[index];
						try
						{
							EditorLog.Log(moved);
							if (File.Exists(moved.to) && !File.Exists(moved.@from))
							{
								requireRefresh = true;
								File.Move(moved.to, moved.@from);
							}
							else if (Directory.Exists(moved.to) && !Directory.Exists(moved.@from))
							{
								requireRefresh = true;
								Directory.CreateDirectory(moved.@from);
							}
						}
						catch (DirectoryNotFoundException ex)
						{
							EditorLog.LogError(moved);
							EditorLog.LogError(ex);
						}
					}

					break;
				case DragAndDropVisualMode.Link:
					break;
				case DragAndDropVisualMode.Move:
					break;
				case DragAndDropVisualMode.Generic:
					break;
			}

			if (requireRefresh)
			{
				AssetDatabase.Refresh();
			}
		}

		protected override void OnUndo()
		{
			if (args.files == null) return;
			var requireRefresh = false;

			if (movedFiles == null) movedFiles = new List<MoveInfo>();
			movedFiles.Clear();
			switch (args.dropResult)
			{
				case DragAndDropVisualMode.None:
					break;
				case DragAndDropVisualMode.Copy:
					for (var index = args.files.Count - 1; index >= 0; index--)
					{
						var file = args.files[index];
						if (File.Exists(file.to))
							MoveFileAndMetaToRecycleBin(file, ref requireRefresh);
					}
					for (var index = args.files.Count - 1; index >= 0; index--)
					{
						var file = args.files[index];
						if (Directory.Exists(file.to))
							MoveFileAndMetaToRecycleBin(file, ref requireRefresh);
					}

					break;
				case DragAndDropVisualMode.Link:
					break;
				case DragAndDropVisualMode.Move:
					break;
				case DragAndDropVisualMode.Generic:
					break;
			}

			if (requireRefresh)
			{
				AssetDatabase.Refresh();
			}
		}

		private void MoveFileAndMetaToRecycleBin(MoveInfo file, ref bool requireRefresh)
		{
			EditorLog.Log(file);
			if (!File.Exists(file.to) && !Directory.Exists(file.to)) return;
			requireRefresh = true;
			if (file.@from == null)
			{
				if (MoveToRecycleBin(file.to))
				{
					var meta = file.to + ".meta";
					if (File.Exists(meta))
					{
						MoveToRecycleBin(meta);
					}
				}
			}
		}

		private bool MoveToRecycleBin(string filePath)
		{
			if (movedFiles == null) movedFiles = new List<MoveInfo>();
			if (RecycleBinHelper.MoveToRecycleBin(filePath, out var res))
			{
				var info = new MoveInfo(filePath, res);
				movedFiles.Add(info);
				EditorLog.Log("<b>moved</b> " + info);
				return true;
			}
			else EditorLog.LogWarning("Failed moving " + filePath);

			return false;
		}
	}
}