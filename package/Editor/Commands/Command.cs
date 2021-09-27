namespace Needle
{
	public abstract class Command : ICommand
	{
		private string name;
		public virtual string Name
		{
			get => string.IsNullOrEmpty(name) ? GetType().Name : name;
			set => name = value;
		}

		public bool CanUndo { get; protected set; } = true;
		public bool CanRedo { get; protected set; } = true;
		public bool IsDone { get; set; } = false;

		bool ICommand.CanUndo() => CanUndo;
		bool ICommand.CanRedo() => CanRedo;
		public virtual bool IsValid { get; } = true;

		protected Command(bool isDone = false)
		{
			this._done = isDone;
		}
		
		void ICommand.PerformRedo()
		{
			if (_done)
			{
				EditorLog.Log("Can not redo, already done " + this);
				return;
			}
			try
			{
				EditorLog.Log("Redo " + this); 
				OnRedo();
			}
			finally
			{
				EditorLog.Log("Redo performed: " + this);
				_done = true;
			}
		}
		void ICommand.PerformUndo()
		{
			if (!_done)
			{
				EditorLog.Log("Can not undo, not done " + this);
				return;
			}
			try
			{
				EditorLog.Log("Undo " + this);
				OnUndo();
			}
			finally
			{
				EditorLog.Log("Undo performed: " + this);
				_done = false;
			}
		}

		protected abstract void OnRedo();
		protected abstract void OnUndo();
		
		private bool _done = false;
	}
}