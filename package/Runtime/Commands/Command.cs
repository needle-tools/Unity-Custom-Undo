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
			// ReSharper disable once VirtualMemberCallInConstructor
			_debugHashCode = GetHashCode();
		}
		
		void ICommand.PerformRedo()
		{
			if (_done)
			{
				UndoLog.Log("Can not redo, already done " + this);
				return;
			}
			try
			{
				UndoLog.Log("Redo " + this); 
				OnRedo();
			}
			finally
			{
				UndoLog.Log("Redo performed: " + this);
				_done = true;
			}
		}
		void ICommand.PerformUndo()
		{
			if (!_done)
			{
				UndoLog.Log("Can not undo, not done " + this);
				return;
			}
			try
			{
				UndoLog.Log("Undo " + this);
				OnUndo();
			}
			finally
			{
				UndoLog.Log("Undo performed: " + this);
				_done = false;
			}
		}

		protected abstract void OnRedo();
		protected abstract void OnUndo();
		
		private bool _done = false;
		private readonly int _debugHashCode;

		public override string ToString()
		{
			return base.ToString() + " (" + _debugHashCode + ")";
		}
	}
}