#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace Needle
{
	public struct MoveInfo
	{
		public string? from;
		public string to;
		
		public MoveInfo(string? @from, string to)
		{
			this.@from = @from;
			this.to = to;
		}

		public override string ToString()
		{
			return "From " + @from + " to " + to;
		}
	}
	
	public readonly struct DropEventArgs
	{
		public readonly DragAndDropVisualMode dropResult;
		public readonly int dragInstanceId;
		public readonly string dropUponPath;
		public readonly List<MoveInfo>? files;

		public DropEventArgs(int dragInstanceId, string dropUponPath, DragAndDropVisualMode dropResult)
		{
			this.dragInstanceId = dragInstanceId;
			this.dropUponPath = dropUponPath;
			this.dropResult = dropResult;
			var list = new List<MoveInfo>();
			files = list;
			switch (dropResult)
			{
				case DragAndDropVisualMode.Copy:
					if (DragDropServiceBridge.lastAddedAssets != null)
					{
						foreach(var added in DragDropServiceBridge.lastAddedAssets)
							list.Add(new MoveInfo(null, added));
					}
					goto case DragAndDropVisualMode.Move;
					
				case DragAndDropVisualMode.Link:
					Debug.Log("link: not implemented?");
					break;
				case DragAndDropVisualMode.Move:
					if (DragDropServiceBridge.lastMovedAssets != null)
					{
						foreach (var moved in DragDropServiceBridge.lastMovedAssets)
							list.Add(new MoveInfo(moved.sourceAssetPath, moved.destinationAssetPath));
					}
					break;
				case DragAndDropVisualMode.Generic:
					Debug.Log("generic: not implemented?");
					break;
			}
		}

		public override string ToString()
		{
			return $"{dropResult} {files?.Count} to {dropUponPath}; {dragInstanceId}";
		}
	}

	internal static class DragDropServiceBridge
	{
		internal static event Action<DropEventArgs>? ProjectBrowserDropPerformed;

		internal static string[]? lastChangedAssets, lastAddedAssets, lastDeletedAssets;
		internal static AssetMoveInfo[]? lastMovedAssets;

		internal class Listener : AssetsModifiedProcessor
		{
			protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
			{
				lastChangedAssets = changedAssets;
				lastAddedAssets = addedAssets;
				lastDeletedAssets = deletedAssets;
				lastMovedAssets = movedAssets;
			}
		}

		[InitializeOnLoadMethod]
		private static void Init()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var asm in assemblies)
			{
				if (asm.FullName == "UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
				{
					var type = asm.GetType("UnityEditor.DragAndDropService");
					if (type != null)
					{
						InitDragDropServiceType(type);
					}
				}
			}
		}

		private static void InitDragDropServiceType(Type type)
		{
			if (didRunInit) return;
			didRunInit = true;
			var fieldInfo = type.GetField("m_DropDescriptors", BindingFlags.Static | BindingFlags.NonPublic);
			dropDescriptorsList = fieldInfo?.GetValue(null) as Dictionary<int, List<Delegate>>;
			if (dropDescriptorsList != null)
			{
				if (dropDescriptorsList.TryGetValue(kProjectBrowserDropDstId, out var list))
				{
					projectBrowserHandler = new List<Delegate>(list);
					list.Clear();
					var del = new Func<int, string, bool, DragAndDropVisualMode>(OnProjectBrowserDrop);
					list.Add(del);
				}
			}
		}

		#region ProjectBrowser

		private static DragAndDropVisualMode OnProjectBrowserDrop(
			int dragInstanceId,
			string dropUponPath,
			bool perform)
		{
			if (projectBrowserHandler == null) return DragAndDropVisualMode.None;
			// Debug.Log(dropUponPath);

			var args = new object[] { dragInstanceId, dropUponPath, perform };
			for (var i = projectBrowserHandler.Count - 1; i >= 0; --i)
			{
				var handler = projectBrowserHandler[i];
				var dropResult = (DragAndDropVisualMode)handler.DynamicInvoke(args);
				if (dropResult != DragAndDropVisualMode.None)
				{
					if (perform)
					{
						if(lastAddedAssets?.Length > 0)
							ProjectBrowserDropPerformed?.Invoke(new DropEventArgs(dragInstanceId, dropUponPath, dropResult));
						lastChangedAssets = lastAddedAssets = lastDeletedAssets = null;
						lastMovedAssets = null;
					}

					return dropResult;
				}
			}

			return DragAndDropVisualMode.Rejected;
		}

		private static readonly int kProjectBrowserDropDstId = "ProjectBrowser".GetHashCode();
		private static List<Delegate>? projectBrowserHandler;

		#endregion

		private static bool didRunInit;
		private static Dictionary<int, List<Delegate>>? dropDescriptorsList;
	}
}