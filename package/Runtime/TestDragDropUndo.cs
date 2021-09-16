﻿using System;
using System.IO;
using Needle;
using UnityEngine;

namespace DefaultNamespace
{
	[ExecuteAlways]
	public class TestDragDropUndo : MonoBehaviour
	{
		private void OnEnable()
		{
			DragDropServiceBridge.ProjectBrowserDropPerformed += OnDrop;
		}

		private void OnDisable()
		{
			DragDropServiceBridge.ProjectBrowserDropPerformed -= OnDrop;
		}

		private void OnDrop(DropEventArgs obj)
		{
			CustomUndo.Register(new DragDropUndo(obj));
		}
	}
}