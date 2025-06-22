using System;
using _SPC.Core.Scripts.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.Core.Scripts.Text
{
    public class NameInputGrid
    {
        public event Action<Vector2Int> OnSelectionChanged;
        public event Action<char> OnCharacterSelected;
        public event Action OnSubmit;
        public event Action OnBackspace;

        public string[] CharacterGrid => _characterGrid;
        private readonly string[] _characterGrid =
        {
            "ABCDEFG",
            "HIJKLMN",
            "OPQRSTU",
            "VWXYZ12",
            "3456789",
            "0<>"
        };
        
        private readonly float _moveCooldown;
        private readonly InputSystem_Actions _inputActions;
        private Vector2Int _currentGridPosition;
        private float _lastMoveTime;

        public NameInputGrid(float moveCooldown)
        {
            _moveCooldown = moveCooldown;
            _inputActions = InputSystemBuffer.Instance.InputSystem;
            _currentGridPosition = Vector2Int.zero;
        }

        public void Enable()
        {
            _inputActions.Player.Move.performed += OnMove;
            _inputActions.Player.Attack.performed += OnAttack;
        }

        public void Disable()
        {
            _inputActions.Player.Move.performed -= OnMove;
            _inputActions.Player.Attack.performed -= OnAttack;
        }

        public void InvokeInitialSelection()
        {
            OnSelectionChanged?.Invoke(_currentGridPosition);
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            char selectedChar = _characterGrid[_currentGridPosition.y][_currentGridPosition.x];

            switch (selectedChar)
            {
                case '<':
                    OnBackspace?.Invoke();
                    break;
                case '>':
                    OnSubmit?.Invoke();
                    break;
                default:
                    OnCharacterSelected?.Invoke(selectedChar);
                    break;
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (Time.unscaledTime - _lastMoveTime < _moveCooldown)
            {
                return;
            }

            Vector2 moveInput = context.ReadValue<Vector2>();

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0) MoveRight();
                else MoveLeft();
            }
            else
            {
                if (moveInput.y > 0) MoveUp();
                else MoveDown();
            }

            _lastMoveTime = Time.unscaledTime;
            OnSelectionChanged?.Invoke(_currentGridPosition);
        }

        private void MoveUp()
        {
            _currentGridPosition.y = (_currentGridPosition.y - 1 + _characterGrid.Length) % _characterGrid.Length;

            if (_currentGridPosition.x >= _characterGrid[_currentGridPosition.y].Length)
            {
                _currentGridPosition.x = _characterGrid[_currentGridPosition.y].Length - 1;
            }
        }

        private void MoveDown()
        {
            _currentGridPosition.y = (_currentGridPosition.y + 1) % _characterGrid.Length;
            
            if (_currentGridPosition.x >= _characterGrid[_currentGridPosition.y].Length)
            {
                _currentGridPosition.x = _characterGrid[_currentGridPosition.y].Length - 1;
            }
        }

        private void MoveLeft()
        {
            _currentGridPosition.x = (_currentGridPosition.x - 1 + _characterGrid[_currentGridPosition.y].Length) % _characterGrid[_currentGridPosition.y].Length;
        }

        private void MoveRight()
        {
            _currentGridPosition.x = (_currentGridPosition.x + 1) % _characterGrid[_currentGridPosition.y].Length;
        }
    }
} 