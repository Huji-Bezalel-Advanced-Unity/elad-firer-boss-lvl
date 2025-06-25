using System;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    public struct BossRageAttackDependencies
    {
        public Transform EntityTransform;
        public Transform MainTarget;
        public GameLogger Logger;
    }

    public class BossRageAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossRageAttackDependencies _deps;
        private bool _isAttacking = false;
        private bool _isPaused = false;
        private Vector3 _originalPosition;
        private Action _onAttackFinished;
        private Sequence _currentSequence;

        public BossRageAttack(BossStats stats, BossRageAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        private void OnGamePaused()
        {
            _isPaused = true;
            if (_isAttacking && _currentSequence != null)
            {
                _currentSequence.Pause();
            }
        }

        private void OnGameResumed()
        {
            _isPaused = false;
            if (_isAttacking && _currentSequence != null)
            {
                _currentSequence.Play();
            }
        }

        
        public override bool Attack(Action onFinished = null)
        {
            if (_isAttacking) return false;
            
            _isAttacking = true;
            _onAttackFinished = onFinished;
            _originalPosition = _deps.EntityTransform.position;

            // Move towards player
            Vector3 targetPosition = _deps.MainTarget.position;
            
            _currentSequence = DOTween.Sequence();

            // Shake before charging
            _currentSequence.Append(_deps.EntityTransform.DOShakePosition(_stats.rageShakeTime, _stats.rageShakeStrength, _stats.rageShakeVibrato, _stats.rageShakeRandomness, false, true)
                .SetEase(_stats.rageShakeEase));

            // Charge towards player
            _currentSequence.Append(_deps.EntityTransform.DOMove(targetPosition, _stats.rageChargeTime)
                .SetEase(_stats.rageChargeEase));

            // Stay at charge position briefly
            _currentSequence.AppendInterval(_stats.rageHoldTime);

            // Return to original position
            _currentSequence.Append(_deps.EntityTransform.DOMove(_originalPosition, _stats.rageReturnTime)
                .SetEase(_stats.rageReturnEase));

            // Execute callback and reset state
            _currentSequence.OnComplete(() =>
            {
                _isAttacking = false;
                _onAttackFinished?.Invoke();
                _onAttackFinished = null;
            });

            // Handle pause/resume
            _currentSequence.SetUpdate(true);
            return true;
        }

        public bool IsAttacking => _isAttacking;

        private void StopAttack()
        {
            if (_isAttacking)
            {
                _currentSequence?.Kill();
                _currentSequence = null;
                _deps.EntityTransform.position = _originalPosition;
                _isAttacking = false;
                _onAttackFinished?.Invoke();
                _onAttackFinished = null;
            }
        }

        public void Cleanup()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
            StopAttack();
        }
    }
} 