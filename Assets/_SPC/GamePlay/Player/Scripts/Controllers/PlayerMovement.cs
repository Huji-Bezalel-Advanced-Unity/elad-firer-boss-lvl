using _SPC.Core.Scripts.InputSystem;
using _SPC.GamePlay.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerMovement
    {
        private readonly Rigidbody2D _rb;
        private readonly PlayerStats _stats;
        private readonly GameLogger _playerLogger;
        private readonly InputSystem_Actions _inputSystem;
        private Vector2 _moveInput = Vector2.zero;
        
        public bool IsMoving { get; private set; }
        private Vector2 _direction;

        public PlayerMovement(Rigidbody2D rb, PlayerStats stats, GameLogger playerLogger) 
        {
            _rb = rb;
            _stats = stats;
            _playerLogger = playerLogger;

            _rb.mass = 1f;
            
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            
            _inputSystem.Player.Move.performed += OnMovePerformed;
            _inputSystem.Player.Move.canceled  += OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            // ReadVector2 gives you the full 2D stick/keyboard composite
            _moveInput = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            // Zero out when the user lets go
            _moveInput = Vector2.zero;
        }

        public void UpdateMovement()
        {
            if (_moveInput == Vector2.zero)
            {
                IsMoving = false;
                return;
            }
            IsMoving = true;
            if (_moveInput.sqrMagnitude < _stats.MovementThreshold * _stats.MovementThreshold)
                return;
            _direction = _moveInput.normalized;
            _rb.AddForce(_direction * (_stats.Acceleration * Time.deltaTime), ForceMode2D.Force);
        }

    }

}