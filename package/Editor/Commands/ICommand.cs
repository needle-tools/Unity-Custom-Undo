namespace Needle
{
	public interface ICommand
	{
		string Name { get; }
		internal void PerformUndo();
		internal void PerformRedo();
	}
}