using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using Core.Input_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerMovement : LBMovement
    {
        private readonly InputSystem_Actions _inputSystem;
        private Vector2 _moveInput = Vector2.zero;

        

        public PlayerMovement(Rigidbody2D rb, LBStats stats) : base(rb, stats)
        {
            Rigidbody.mass = 1f;
            
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

        public override void UpdateMovement()
        {
            if (_moveInput.sqrMagnitude < Stats.MovementThreshold * Stats.MovementThreshold)
                return;

            Direction = _moveInput.normalized;
            Rigidbody.AddForce(Direction * (Stats.Accelartion * Time.deltaTime), ForceMode2D.Force);
        }
        
}

}