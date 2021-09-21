using System.Collections.Generic;
using UnityEngine;

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
			if (_currentIndex < 0 || _commands.Count <= 0)
			{
				EditorLog.Log("Nothing to undo");
				return false;
			}

			if (_currentIndex >= _commands.Count) _currentIndex = _commands.Count - 1;
			var cmd = _commands[_currentIndex];
			_currentIndex -= 1;
			if (!cmd.CanUndo())
			{
				EditorLog.Log("Can not undo " + cmd);
				return false;
			}
			EditorLog.Log("Undo " + cmd);
			cmd.PerformUndo();
			if (_currentIndex < 0) _currentIndex = 0;
			return false;
		}

		public bool Redo()
		{
			if (_commands.Count <= 0)
			{
				EditorLog.Log("Nothing to redo");
				return false;
			}
			
			if (IsAtHead)
			{
				EditorLog.Log("Nothing to redo (is at head)");
				return false;
			}
			
			var cmd = _commands[_currentIndex++];
			if (!cmd.CanRedo())
			{
				EditorLog.Log("Can not redo " + cmd);
				return false;
			}
			EditorLog.Log("Redo " + cmd);
			cmd.PerformRedo();
			return true;
		}

		public void Clear()
		{
			_commands.Clear();
			_currentIndex = 0;
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
			if (_commands.Contains(command))
			{
				Debug.LogError("Command is already in queue!");
				return false;
			}
			RemoveCommandAfterIndexIfNecessary();
			
			while (_commands.Count >= _maxLength)
			{
				_commands.RemoveAt(0);
			}

			EditorLog.Log("Add command: " + command);
			command.PerformRedo();
			_commands.Add(command);
			_currentIndex = _commands.Count - 1;
			return true;
		}

		private bool IsAtHead => _currentIndex >= 0 && 
		                         (_currentIndex == _commands.Count || _commands.Count <= 0);

		private uint _maxLength;
		private int _currentIndex;
		private readonly List<ICommand> _commands = new List<ICommand>();
		internal IReadOnlyList<ICommand> Commands => _commands;
	}
}