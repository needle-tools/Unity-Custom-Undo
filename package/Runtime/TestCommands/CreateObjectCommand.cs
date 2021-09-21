using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Needle
{
	// stupid example but oh well
	public class CreateObjectCommand : Command
	{
		private readonly GameObject _prefab;
		private GameObject _instance;

		private readonly string id; 

		public GameObject Instance => _instance;

		public CreateObjectCommand(GameObject prefab)
		{
			this._prefab = prefab;
			this.id = DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		protected override void OnRedo()
		{
			if (!_prefab) return;
			if(!_instance) _instance = Object.Instantiate(_prefab);
			_instance.hideFlags = HideFlags.DontSaveInEditor;
			if(!_instance.activeSelf)
				_instance.SetActive(true);
		}

		protected override void OnUndo()
		{
			if (!_instance) return;
			_instance.SetActive(false);
			_instance.hideFlags = HideFlags.HideAndDontSave;
		}

		public override string ToString()
		{
			return base.ToString() + "-" + id;
		}
	}
}