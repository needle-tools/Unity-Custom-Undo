using Codice.Client.ChangeTrackerService;
using Needle;
using Needle.InspectorButton;
using UnityEditorInternal;
using UnityEngine;

public class TestCommandEmitter : MonoBehaviour
{
	public ButtonBase EmitTestCommand = new InspectorButton<TestCommandEmitter>(
		i =>
		{
			var cmd = new TestCommand();
			UndoEverything.Register(cmd);
		}){height = 40};



	public GameObject Prefab;
	private CreateObjectCommand _lastCreateObject;
	
	public ButtonBase CreateDummyPrefab = new InspectorButton<TestCommandEmitter>(
		i =>
		{
			if (!i.Prefab) return;
			var cmd = new CreateObjectCommand(i.Prefab);
			i._lastCreateObject = cmd;
			UndoEverything.Register(cmd);
			InternalEditorUtility.RepaintAllViews();
		}){height = 40};

	public ButtonBase RotateLastCreatedObject = new InspectorButton<TestCommandEmitter>(
		i =>
		{
			if (!i.Prefab) return;
			var cmd = new RotateObjectCommand(i._lastCreateObject);
			UndoEverything.Register(cmd);
			InternalEditorUtility.RepaintAllViews();
		}){height = 40};

	public ButtonBase CreateDummyPrefabAndRotate = new InspectorButton<TestCommandEmitter>(
		i =>
		{
			if (!i.Prefab) return;
			var create = new CreateObjectCommand(i.Prefab);
			var rotate = new RotateObjectCommand(create);
			UndoEverything.Register(new CompoundCommand(create, rotate));
			InternalEditorUtility.RepaintAllViews();
		}){height = 40};
		
		
		
}