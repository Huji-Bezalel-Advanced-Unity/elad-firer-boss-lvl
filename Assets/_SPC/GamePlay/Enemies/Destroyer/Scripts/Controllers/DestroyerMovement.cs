using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.Destroyer.Scripts.Controllers
{
    public struct DestroyerMovementDependencies
    {
        public Transform EntityTransform;
        public DestroyerStats Stats;
        public BoxCollider2D ArenaBounds;
    }

    public class DestroyerMovement
    {
        private readonly Transform _entityTransform;
        private readonly DestroyerStats _stats;
        private readonly BoxCollider2D _arenaBounds;
        private bool _isMoving;

        public DestroyerMovement(DestroyerMovementDependencies deps)
        {
            _entityTransform = deps.EntityTransform;
            _stats = deps.Stats;
            _arenaBounds = deps.ArenaBounds;
            _isMoving = false;
        }

        public void UpdateMovement()
        {
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

            _entityTransform.DOMove(randomPoint, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(Random.Range(1f, 3f), () =>
                    {
                        _isMoving = false;
                    });
                });
        }
    }
} 