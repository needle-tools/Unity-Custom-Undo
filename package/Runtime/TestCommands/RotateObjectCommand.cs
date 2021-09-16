using UnityEngine;

namespace Needle
{
	public class RotateObjectCommand : Command
	{
		private readonly Transform t;
		private readonly Vector3 angle;
		private readonly CreateObjectCommand cmd;

		public RotateObjectCommand(CreateObjectCommand cmd, Vector3? angles = null)
		{
			this.cmd = cmd;
			this.angle = angles ?? new Vector3(0, 45 * Random.value, 0);
		}

		public RotateObjectCommand(Transform t, Vector3? angles = null)
		{
			this.t = t;
			this.angle = angles ?? new Vector3(0, 45 * Random.value, 0);
		}

		protected override void OnRedo()
		{
			if (t)
				t.Rotate(angle);
			else if (cmd != null && cmd.Instance)
				cmd.Instance.transform.Rotate(angle);
		}

		protected override void OnUndo()
		{
			if (t)
				t.Rotate(-angle);
			else if (cmd != null && cmd.Instance)
				cmd.Instance.transform.Rotate(-angle);
		}
	}
}