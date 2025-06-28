using System.Collections.Generic;
using _SPC.Core.BaseScripts.InputSystem.Scripts;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// Holds dependencies for the PlayerMovement system initialization.
    /// </summary>
    public struct PlayerMovementDependencies
    {
        public Rigidbody2D Rb;
        public PlayerStats Stats;
        public GameLogger PlayerLogger;
        public Transform SpaceshipTransform;
        public List<Transform> TransformTargets;
    }

    /// <summary>
    /// Handles player movement logic, including input processing, physics-based movement, and rotation.
    /// </summary>
    public sealed class PlayerMovement : SPCMovement
    {
        private readonly Rigidbody2D _rb;
        private readonly PlayerStats _stats;
        private readonly GameLogger _playerLogger;
        private  InputSystem_Actions _inputSystem;
        private Vector2 _moveInput = Vector2.zero;
        private Vector2 _savedVelocity;
        private float _savedAngularVelocity;
        private bool _isPaused = false;
        
        private Vector2 _direction;
        private readonly Transform _spaceshipTransform;
        private readonly List<Transform> _transformTargets;

        /// <summary>
        /// Initializes the player movement system with dependencies.
        /// </summary>
        /// <param name="deps">Dependencies required for movement initialization.</param>
        public PlayerMovement(PlayerMovementDependencies deps) 
        {
            _rb = deps.Rb;
            _stats = deps.Stats;
            _playerLogger = deps.PlayerLogger;
            _spaceshipTransform = deps.SpaceshipTransform;
            _transformTargets = deps.TransformTargets;

            
            InitializeInputSystem();
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Updates the player movement each frame.
        /// </summary>
        public override void UpdateMovement()
        {
            if (ShouldSkipMovement())
            {
                IsMoving = false;
                return;
            }
            
            IsMoving = true;
            
            if (IsBelowMovementThreshold())
                return;
                
            UpdateMovementDirection();
            ApplyMovementForce();
            RotateTowardsNearestTarget();
        }

        /// <summary>
        /// Cleans up event subscriptions when the movement system is destroyed.
        /// </summary>
        public override void Cleanup()
        {
            UnsubscribeFromGameEvents();
            UnsubscribeFromInputEvents();
        }

        /// <summary>
        /// Handles game pause event by saving physics state and freezing the rigidbody.
        /// </summary>
        private void OnGamePaused()
        {
            _isPaused = true;
            SavePhysicsState();
            FreezeRigidbody();
        }

        /// <summary>
        /// Handles game resume event by restoring physics state and unfreezing the rigidbody.
        /// </summary>
        private void OnGameResumed()
        {
            _isPaused = false;
            UnfreezeRigidbody();
            RestorePhysicsState();
        }

        /// <summary>
        /// Handles movement input from the player.
        /// </summary>
        /// <param name="ctx">Input action callback context.</param>
        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput = ctx.ReadValue<Vector2>();
        }

        /// <summary>
        /// Handles movement input cancellation from the player.
        /// </summary>
        /// <param name="ctx">Input action callback context.</param>
        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }

        /// <summary>
        /// Rotates the spaceship towards the nearest target.
        /// </summary>
        private void RotateTowardsNearestTarget()
        {
            if (!HasValidTargets()) return;
            
            Transform closest = GetClosestTarget();
            if (closest == null) return;

            Vector3 direction = CalculateDirectionToTarget(closest);
            if (IsDirectionTooSmall(direction)) return;
            
            RotateSpaceship(direction);
        }

        

        /// <summary>
        /// Initializes the input system and subscribes to movement events.
        /// </summary>
        private void InitializeInputSystem()
        {
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Move.performed += OnMovePerformed;
            _inputSystem.Player.Move.canceled += OnMoveCanceled;
        }

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        /// <summary>
        /// Unsubscribes from game pause/resume events.
        /// </summary>
        private void UnsubscribeFromGameEvents()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
        }

        /// <summary>
        /// Unsubscribes from input events.
        /// </summary>
        private void UnsubscribeFromInputEvents()
        {
            _inputSystem.Player.Move.performed -= OnMovePerformed;
            _inputSystem.Player.Move.canceled -= OnMoveCanceled;
        }

        /// <summary>
        /// Determines if movement should be skipped based on pause state or lack of input.
        /// </summary>
        /// <returns>True if movement should be skipped, false otherwise.</returns>
        private bool ShouldSkipMovement()
        {
            return _isPaused || _moveInput == Vector2.zero;
        }

        /// <summary>
        /// Checks if the movement input is below the movement threshold.
        /// </summary>
        /// <returns>True if below threshold, false otherwise.</returns>
        private bool IsBelowMovementThreshold()
        {
            return _moveInput.sqrMagnitude < _stats.MovementThreshold * _stats.MovementThreshold;
        }

        /// <summary>
        /// Updates the movement direction based on input.
        /// </summary>
        private void UpdateMovementDirection()
        {
            _direction = _moveInput.normalized;
        }

        /// <summary>
        /// Applies movement force to the rigidbody.
        /// </summary>
        private void ApplyMovementForce()
        {
            _rb.AddForce(_direction * (_stats.Acceleration * Time.deltaTime), ForceMode2D.Force);
        }

        /// <summary>
        /// Checks if there are valid targets available.
        /// </summary>
        /// <returns>True if targets are available, false otherwise.</returns>
        private bool HasValidTargets()
        {
            return _transformTargets != null && _transformTargets.Count > 0;
        }

        /// <summary>
        /// Gets the closest target from the available targets.
        /// </summary>
        /// <returns>The closest target transform, or null if no targets available.</returns>
        private Transform GetClosestTarget()
        {
            return UsedAlgorithms.GetClosestTarget(_transformTargets, _spaceshipTransform);
        }

        /// <summary>
        /// Calculates the direction vector to the target.
        /// </summary>
        /// <param name="target">The target transform.</param>
        /// <returns>The normalized direction vector.</returns>
        private Vector3 CalculateDirectionToTarget(Transform target)
        {
            return (target.position - _spaceshipTransform.position).normalized;
        }

        /// <summary>
        /// Checks if the direction vector is too small to be meaningful.
        /// </summary>
        /// <param name="direction">The direction vector to check.</param>
        /// <returns>True if direction is too small, false otherwise.</returns>
        private bool IsDirectionTooSmall(Vector3 direction)
        {
            return direction.sqrMagnitude < 0.0001f;
        }

        /// <summary>
        /// Rotates the spaceship to face the specified direction.
        /// </summary>
        /// <param name="direction">The direction to face.</param>
        private void RotateSpaceship(Vector3 direction)
        {
            _spaceshipTransform.up = -direction;
        }

        /// <summary>
        /// Saves the current physics state for later restoration.
        /// </summary>
        private void SavePhysicsState()
        {
            if (_rb != null)
            {
                _savedVelocity = _rb.linearVelocity;
                _savedAngularVelocity = _rb.angularVelocity;
            }
        }

        /// <summary>
        /// Freezes the rigidbody by setting it to static.
        /// </summary>
        private void FreezeRigidbody()
        {
            if (_rb != null)
            {
                _rb.bodyType = RigidbodyType2D.Static;
            }
        }

        /// <summary>
        /// Unfreezes the rigidbody by setting it to dynamic.
        /// </summary>
        private void UnfreezeRigidbody()
        {
            if (_rb != null)
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        /// <summary>
        /// Restores the saved physics state.
        /// </summary>
        private void RestorePhysicsState()
        {
            if (_rb != null)
            {
                _rb.linearVelocity = _savedVelocity;
                _rb.angularVelocity = _savedAngularVelocity;
            }
        }
    }
}