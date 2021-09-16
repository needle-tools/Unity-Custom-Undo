﻿namespace Needle
{
	public abstract class Command : ICommand
	{
		public virtual string Name => GetType().Name;

		public bool CanUndo { get; protected set; } = true;
		public bool CanRedo { get; protected set; } = true;
		public bool IsDone { get; protected set; } = false;

		protected Command(bool isDone = false)
		{
			this._done = isDone;
		}
		
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