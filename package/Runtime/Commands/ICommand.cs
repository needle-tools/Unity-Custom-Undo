namespace Needle
{
	public interface ICommand
	{
		string Name { get; }
		void PerformUndo();
		void PerformRedo();
		internal bool CanUndo();
		internal bool CanRedo();
		bool IsValid { get; }
	}
}