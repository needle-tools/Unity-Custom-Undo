namespace Needle
{
	public abstract class Command : ICommand
	{
		public virtual string Name => GetType().Name;
		
		void ICommand.PerformRedo()
		{
			if (_done) return;
			try
			{
				OnRedo();
			}
			finally
			{
				_done = true;
			}
		}

		void ICommand.PerformUndo()
		{
			if (!_done) return;
			try
			{
				OnUndo();
			}
			finally
			{
				_done = false;
			}
		}

		protected abstract void OnRedo();
		protected abstract void OnUndo();
		
		private bool _done = false;
	}
}