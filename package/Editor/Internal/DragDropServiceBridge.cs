using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Needle
{
	internal static class DragDropServiceBridge
	{
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
		private static DragAndDropVisualMode OnProjectBrowserDrop(int dragInstanceId, string dropUponPath, bool perform)
		{
			Debug.Log(dropUponPath);
			
			var args = new object[] { dragInstanceId, dropUponPath, perform };
			for (var i = projectBrowserHandler.Count - 1; i >= 0; --i)
			{
				var handler = projectBrowserHandler[i];
				var dropResult = (DragAndDropVisualMode)handler.DynamicInvoke(args);
				if (dropResult != DragAndDropVisualMode.None)
				{
					if (perform) Debug.Log("DROPPED TO " + dropUponPath);
					return dropResult;
				}
			}

			return DragAndDropVisualMode.Rejected;
		}

		private static readonly int kProjectBrowserDropDstId = "ProjectBrowser".GetHashCode();
		private static List<Delegate> projectBrowserHandler;
		#endregion
		
		private static bool didRunInit;
		private static Dictionary<int, List<Delegate>> dropDescriptorsList;
	}
}