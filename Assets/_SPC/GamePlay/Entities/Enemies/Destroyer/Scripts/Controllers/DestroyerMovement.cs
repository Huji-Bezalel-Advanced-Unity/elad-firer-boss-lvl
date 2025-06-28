using System.Collections.Generic;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Destroyer
{
    /// <summary>
    /// Holds dependencies for the DestroyerMovement, including transforms, stats, and arena bounds.
    /// </summary>
    public struct DestroyerMovementDependencies
    {
        public Transform EntityTransform;
        public DestroyerStats Stats;
        public BoxCollider2D ArenaBounds;
        public Transform SpaceshipTransform;
        public List<Transform> TransformTargets;
    }

    /// <summary>
    /// Handles the destroyer's movement logic including wandering and target tracking.
    /// </summary>
    public class DestroyerMovement: SPCMovement
    {
        private readonly Transform _entityTransform;
        private readonly DestroyerStats _stats;
        private readonly BoxCollider2D _arenaBounds;
        private readonly Transform _spaceshipTransform;
        private readonly List<Transform> _transformTargets;

        /// <summary>
        /// Initializes the DestroyerMovement with dependencies and subscribes to game events.
        /// </summary>
        public DestroyerMovement(DestroyerMovementDependencies deps)
        {
            _entityTransform = deps.EntityTransform;
            _stats = deps.Stats;
            _arenaBounds = deps.ArenaBounds;
            _spaceshipTransform = deps.SpaceshipTransform;
            _transformTargets = deps.TransformTargets;
            IsMoving = false;
            
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Updates the destroyer's movement and rotation logic.
        /// </summary>
        public override void UpdateMovement()
        {
            RotateTowardsNearestTarget();

            if (IsMoving) return;
            
            IsMoving = true;
            MoveToNewPoint();
        }

        /// <summary>
        /// Cleans up event subscriptions and DOTween animations.
        /// </summary>
        public override void Cleanup()
        {
            UnsubscribeFromGameEvents();
            KillDOTweenAnimations();
        }

        /// <summary>
        /// Handles game pause events by pausing DOTween animations.
        /// </summary>
        private void OnGamePaused()
        {
            DOTween.Pause(_entityTransform);
        }

        /// <summary>
        /// Handles game resume events by resuming DOTween animations.
        /// </summary>
        private void OnGameResumed()
        {
            DOTween.Play(_entityTransform);
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
        /// Unsubscribes from game events.
        /// </summary>
        private void UnsubscribeFromGameEvents()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
        }

        /// <summary>
        /// Kills all DOTween animations for this entity.
        /// </summary>
        private void KillDOTweenAnimations()
        {
            DOTween.Kill(_entityTransform);
        }

        /// <summary>
        /// Moves the destroyer to a random point within the arena bounds.
        /// </summary>
        private void MoveToNewPoint()
        {
            var bounds = _arenaBounds.bounds;
            Vector2 randomPoint = GenerateRandomPoint(bounds);
            float duration = CalculateMovementDuration(randomPoint);

            CreateMovementSequence(randomPoint, duration);
        }

        /// <summary>
        /// Generates a random point within the arena bounds.
        /// </summary>
        private Vector2 GenerateRandomPoint(Bounds bounds)
        {
            return new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        }

        /// <summary>
        /// Calculates the movement duration based on distance and speed.
        /// </summary>
        private float CalculateMovementDuration(Vector2 targetPoint)
        {
            return Vector2.Distance(_entityTransform.position, targetPoint) / _stats.MovementSpeed;
        }

        /// <summary>
        /// Creates the movement sequence with movement and delay.
        /// </summary>
        private void CreateMovementSequence(Vector2 targetPoint, float duration)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_entityTransform.DOMove(targetPoint, duration).SetEase(Ease.Linear));
            sequence.AppendInterval(GetRandomWanderDelay());
            sequence.OnComplete(() => IsMoving = false);
            sequence.SetTarget(_entityTransform);
        }

        /// <summary>
        /// Gets a random wander delay between min and max values.
        /// </summary>
        private float GetRandomWanderDelay()
        {
            return Random.Range(_stats.minWanderDelay, _stats.maxWanderDelay);
        }

        /// <summary>
        /// Rotates the spaceship towards the nearest target.
        /// </summary>
        private void RotateTowardsNearestTarget()
        {
            if (!HasValidTargets()) return;
            
            Transform closest = FindClosestTarget();
            if (closest == null) return;

            Vector3 direction = CalculateDirectionToTarget(closest);
            if (IsValidDirection(direction))
            {
                RotateSpaceship(direction);
            }
        }

        /// <summary>
        /// Checks if there are valid targets to rotate towards.
        /// </summary>
        private bool HasValidTargets()
        {
            return _transformTargets != null && _transformTargets.Count > 0;
        }

        /// <summary>
        /// Finds the closest target from the available targets.
        /// </summary>
        private Transform FindClosestTarget()
        {
            return UsedAlgorithms.GetClosestTarget(_transformTargets, _spaceshipTransform);
        }

        /// <summary>
        /// Calculates the direction vector to the target.
        /// </summary>
        private Vector3 CalculateDirectionToTarget(Transform target)
        {
            return (target.position - _spaceshipTransform.position).normalized;
        }

        /// <summary>
        /// Checks if the direction vector is valid (not too small).
        /// </summary>
        private bool IsValidDirection(Vector3 direction)
        {
            return direction.sqrMagnitude >= 0.0001f;
        }

        /// <summary>
        /// Rotates the spaceship to face the given direction.
        /// </summary>
        private void RotateSpaceship(Vector3 direction)
        {
            _spaceshipTransform.up = -direction;
        }
    }
} 