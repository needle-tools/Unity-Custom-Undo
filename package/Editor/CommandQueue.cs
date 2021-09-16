﻿using System.Collections.Generic;
using NUnit.Framework;

namespace Needle.UndoEverything
{
	internal class CommandQueue
	{
		public CommandQueue(uint maxLength = 500)
		{
			_maxLength = maxLength;
		}

		public bool Enqueue(ICommand command)
		{
			return OnAddCommand(command);
		}

		public void Undo()
		{
			if (_currentIndex < 0 || _commands.Count <= 0) return;
			var cmd = _commands[_currentIndex];
			cmd.PerformUndo();
			_currentIndex -= 1;
			if (_currentIndex < 0) _currentIndex = 0;
		}

		public void Redo()
		{
			if (_commands.Count <= 0) return;
			if (IsAtHead) return;
			_currentIndex += 1;
			var cmd = _commands[_currentIndex];
			cmd.PerformRedo();
		}

		private void RemoveCommandAfterIndexIfNecessary()
		{
			if (!IsAtHead)
			{
				for (var i = _commands.Count - 1; i > _currentIndex; i--)
				{
					_commands.RemoveAt(i);
				}
			}
		}

		private bool OnAddCommand(ICommand command)
		{
			if (command == null) return false;
			if (_commands.Contains(command)) return false;

			RemoveCommandAfterIndexIfNecessary();
			
			while (_commands.Count >= _maxLength)
			{
				_commands.RemoveAt(0);
			}

			command.PerformRedo();
			_commands.Add(command);
			_currentIndex = _commands.Count - 1;
			return true;
		}

		private bool IsAtHead => _currentIndex >= 0 && (_currentIndex == _commands.Count - 1 || _commands.Count <= 0);

		private uint _maxLength;
		private int _currentIndex;
		private readonly List<ICommand> _commands = new List<ICommand>();
	}

	public interface ICommand
	{
		string Name { get; }
		internal void PerformUndo();
		internal void PerformRedo();
	}

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