using UnityEngine;

namespace Needle
{
	// stupid example but oh well
	public class CreateObjectCommand : Command
	{
		private readonly GameObject _prefab;
		private GameObject _instance;

		public GameObject Instance => _instance;

		public CreateObjectCommand(GameObject prefab)
		{
			this._prefab = prefab;
		}

		protected override void OnRedo()
		{
			if (!_prefab) return;
			_instance = Object.Instantiate(_prefab);
			_instance.hideFlags = HideFlags.DontSave;
		}

		protected override void OnUndo()
		{
			if (!_instance) return;
			if (Application.isPlaying)
				Object.Destroy(_instance);
			else Object.DestroyImmediate(_instance);
			_instance = null;
		}
	}
}