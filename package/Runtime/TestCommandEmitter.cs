using Needle.InspectorButton;
using Needle.UndoEverything;
using UnityEngine;

public class TestCommandEmitter : MonoBehaviour
{
	public ButtonBase Button = new InspectorButton<TestCommandEmitter>(
		i =>
		{
			var cmd = new TestCommand();
			UndoEverything.Register(cmd);
		});
}