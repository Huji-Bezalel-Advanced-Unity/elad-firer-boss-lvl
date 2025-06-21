using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Managers;

namespace _SPC.GamePlay.Enemies.Destroyer.Scripts.Controllers
{
    public struct DestroyerMovementDependencies
    {
        public Transform EntityTransform;
        public DestroyerStats Stats;
        public BoxCollider2D ArenaBounds;
        public Transform SpaceshipTransform;
        public List<Transform> TransformTargets;
    }

    public class DestroyerMovement
    {
        private readonly Transform _entityTransform;
        private readonly DestroyerStats _stats;
        private readonly BoxCollider2D _arenaBounds;
        private readonly Transform _spaceshipTransform;
        private readonly List<Transform> _transformTargets;
        private bool _isMoving;

        public DestroyerMovement(DestroyerMovementDependencies deps)
        {
            _entityTransform = deps.EntityTransform;
            _stats = deps.Stats;
            _arenaBounds = deps.ArenaBounds;
            _spaceshipTransform = deps.SpaceshipTransform;
            _transformTargets = deps.TransformTargets;
            _isMoving = false;
            
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        private void OnGamePaused()
        {
            DOTween.Pause(_entityTransform);
        }

        private void OnGameResumed()
        {
            DOTween.Play(_entityTransform);
        }

        public void UpdateMovement()
        {
            RotateTowardsNearestTarget();

            if (_isMoving) return;
            
            _isMoving = true;
            MoveToNewPoint();
        }

        private void MoveToNewPoint()
        {
            var bounds = _arenaBounds.bounds;
            Vector2 randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            float duration = Vector2.Distance(_entityTransform.position, randomPoint) / _stats.MovementSpeed;

            var sequence = DOTween.Sequence();
            sequence.Append(_entityTransform.DOMove(randomPoint, duration).SetEase(Ease.Linear));
            sequence.AppendInterval(Random.Range(_stats.minWanderDelay, _stats.maxWanderDelay));
            sequence.OnComplete(() => _isMoving = false);
            sequence.SetTarget(_entityTransform);
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
        
        public void Cleanup()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            DOTween.Kill(_entityTransform);
        }
    }
} 