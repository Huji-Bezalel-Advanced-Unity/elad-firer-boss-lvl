using _SPC.Core.Scripts.InputSystem;
using _SPC.GamePlay.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using _SPC.GamePlay.Managers;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public struct PlayerMovementDependencies
    {
        public Rigidbody2D Rb;
        public PlayerStats Stats;
        public GameLogger PlayerLogger;
        public Transform SpaceshipTransform;
        public List<Transform> TransformTargets;
    }

    public sealed class PlayerMovement
    {
        private readonly Rigidbody2D _rb;
        private readonly PlayerStats _stats;
        private readonly GameLogger _playerLogger;
        private readonly InputSystem_Actions _inputSystem;
        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _savedVelocity;
        private float _savedAngularVelocity;
        private bool _isPaused = false;
        
        public bool IsMoving { get; private set; }
        private Vector2 _direction;
        private readonly Transform _spaceshipTransform;
        private readonly List<Transform> _transformTargets;

        public PlayerMovement(PlayerMovementDependencies deps) 
        {
            _rb = deps.Rb;
            _stats = deps.Stats;
            _playerLogger = deps.PlayerLogger;
            _spaceshipTransform = deps.SpaceshipTransform;
            _transformTargets = deps.TransformTargets;

            _rb.mass = 1f;
            
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            
            _inputSystem.Player.Move.performed += OnMovePerformed;
            _inputSystem.Player.Move.canceled  += OnMoveCanceled;

            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        private void OnGamePaused()
        {
            _isPaused = true;
            if (_rb != null)
            {
                _savedVelocity = _rb.linearVelocity;
                _savedAngularVelocity = _rb.angularVelocity;
                _rb.bodyType = RigidbodyType2D.Static;
            }
        }

        private void OnGameResumed()
        {
            _isPaused = false;
            if (_rb != null)
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
                _rb.linearVelocity = _savedVelocity;
                _rb.angularVelocity = _savedAngularVelocity;
            }
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
            if (_isPaused || _moveInput == Vector2.zero)
            {
                IsMoving = false;
                return;
            }
            IsMoving = true;
            if (_moveInput.sqrMagnitude < _stats.MovementThreshold * _stats.MovementThreshold)
                return;
            _direction = _moveInput.normalized;
            _rb.AddForce(_direction * (_stats.Acceleration * Time.deltaTime), ForceMode2D.Force);

            RotateTowardsNearestTarget();
        }

        private void RotateTowardsNearestTarget()
        {
            if (_transformTargets == null || _transformTargets.Count == 0) return;
            Transform closest = UsedAlgorithms.GetClosestTarget(_transformTargets, _spaceshipTransform);
            if (closest == null) return;

            Vector3 dir = (closest.position - _spaceshipTransform.position).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;
            
            _spaceshipTransform.up = -dir;
        }
    }
}