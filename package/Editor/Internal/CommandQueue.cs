using System.Collections.Generic;

namespace Needle
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

		public bool Undo()
		{
			if (_currentIndex < 0 || _commands.Count <= 0) return false;
			var cmd = _commands[_currentIndex];
			if (!cmd.CanUndo()) return false;
			cmd.PerformUndo();
			_currentIndex -= 1;
			if (_currentIndex < 0) _currentIndex = 0;
			return false;
		}

		public bool Redo()
		{
			if (_commands.Count <= 0) return false;
			if (IsAtHead) return false;
			_currentIndex += 1;
			var cmd = _commands[_currentIndex];
			if (!cmd.CanRedo()) return false;
			cmd.PerformRedo();
			return true;
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
}