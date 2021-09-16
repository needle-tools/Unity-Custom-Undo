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
			Debug.Log(args);
		}

		protected override void OnRedo()
		{
			Debug.Log(movedFiles?.Count);
			if (movedFiles == null || movedFiles.Count <= 0) return;
			var requireRefresh = false;
			switch (args.dropResult)
			{
				case DragAndDropVisualMode.Copy:
					for (var index = 0; index < movedFiles.Count; index++)
					{
						var moved = movedFiles[index];
						Debug.Log(moved);
						if (File.Exists(moved.to) && !File.Exists(moved.@from))
						{
							requireRefresh = true;
							File.Move(moved.to, moved.@from);
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
						{
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

		private bool MoveToRecycleBin(string filePath)
		{
			if (movedFiles == null) movedFiles = new List<MoveInfo>();
			if (RecycleBinHelper.MoveToRecycleBin(filePath, out var res))
			{
				var info = new MoveInfo(filePath, res);
				movedFiles.Add(info);
				Debug.Log(info);
				return true;
			}

			return false;
		}
	}
}