using System;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using DG.Tweening;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossRageAttack, including transforms and logger.
    /// </summary>
    public struct BossRageAttackDependencies
    {
        public Transform EntityTransform;
        public Transform MainTarget;
        public GameLogger Logger;
    }

    /// <summary>
    /// Handles the boss's rage attack that charges towards the player and returns.
    /// </summary>
    public class BossRageAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossRageAttackDependencies _deps;
        private bool _isAttacking = false;
        private bool _isPaused = false;
        private Vector3 _originalPosition;
        private Action _onAttackFinished;
        private Sequence _currentSequence;

        /// <summary>
        /// Initializes the BossRageAttack with stats and dependencies.
        /// </summary>
        public BossRageAttack(BossStats stats, BossRageAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Handles game pause events by pausing the attack sequence.
        /// </summary>
        private void OnGamePaused()
        {
            _isPaused = true;
            PauseAttackSequence();
        }

        /// <summary>
        /// Handles game resume events by resuming the attack sequence.
        /// </summary>
        private void OnGameResumed()
        {
            _isPaused = false;
            ResumeAttackSequence();
        }

        /// <summary>
        /// Executes the rage attack sequence.
        /// </summary>
        public override bool Attack(Action onFinished = null)
        {
            if (_isAttacking) return false;
            
            _isAttacking = true;
            _onAttackFinished = onFinished;
            _originalPosition = _deps.EntityTransform.position;

            CreateRageSequence();
            return true;
        }

        /// <summary>
        /// Returns true if the boss is currently performing a rage attack.
        /// </summary>
        public bool IsAttacking => _isAttacking;

        /// <summary>
        /// Stops the current attack and resets the boss position.
        /// </summary>
        private void StopAttack()
        {
            if (_isAttacking)
            {
                KillCurrentSequence();
                ResetBossPosition();
                CompleteAttack();
            }
        }

        /// <summary>
        /// Cleans up event subscriptions and stops any ongoing attack.
        /// </summary>
        public void Cleanup()
        {
            UnsubscribeFromGameEvents();
            StopAttack();
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
        /// Pauses the current attack sequence if it exists.
        /// </summary>
        private void PauseAttackSequence()
        {
            if (_isAttacking && _currentSequence != null)
            {
                _currentSequence.Pause();
            }
        }

        /// <summary>
        /// Resumes the current attack sequence if it exists.
        /// </summary>
        private void ResumeAttackSequence()
        {
            if (_isAttacking && _currentSequence != null)
            {
                _currentSequence.Play();
            }
        }

        /// <summary>
        /// Creates the complete rage attack sequence.
        /// </summary>
        private void CreateRageSequence()
        {
            Vector3 targetPosition = _deps.MainTarget.position;
            
            _currentSequence = DOTween.Sequence();

            AddShakePhase();
            AddChargePhase(targetPosition);
            AddHoldPhase();
            AddReturnPhase();
            AddCompletionCallback();

            _currentSequence.SetUpdate(true);
        }

        /// <summary>
        /// Adds the shake phase to the attack sequence.
        /// </summary>
        private void AddShakePhase()
        {
            _currentSequence.Append(_deps.EntityTransform.DOShakePosition(
                _stats.rageShakeTime, 
                _stats.rageShakeStrength, 
                _stats.rageShakeVibrato, 
                _stats.rageShakeRandomness, 
                false, 
                true
            ).SetEase(_stats.rageShakeEase));
        }

        /// <summary>
        /// Adds the charge phase to the attack sequence.
        /// </summary>
        private void AddChargePhase(Vector3 targetPosition)
        {
            _currentSequence.Append(_deps.EntityTransform.DOMove(targetPosition, _stats.rageChargeTime)
                .SetEase(_stats.rageChargeEase));
        }

        /// <summary>
        /// Adds the hold phase to the attack sequence.
        /// </summary>
        private void AddHoldPhase()
        {
            _currentSequence.AppendInterval(_stats.rageHoldTime);
        }

        /// <summary>
        /// Adds the return phase to the attack sequence.
        /// </summary>
        private void AddReturnPhase()
        {
            _currentSequence.Append(_deps.EntityTransform.DOMove(_originalPosition, _stats.rageReturnTime)
                .SetEase(_stats.rageReturnEase));
        }

        /// <summary>
        /// Adds the completion callback to the attack sequence.
        /// </summary>
        private void AddCompletionCallback()
        {
            _currentSequence.OnComplete(CompleteAttack);
        }

        /// <summary>
        /// Kills the current sequence and sets it to null.
        /// </summary>
        private void KillCurrentSequence()
        {
            _currentSequence?.Kill();
            _currentSequence = null;
        }

        /// <summary>
        /// Resets the boss position to the original position.
        /// </summary>
        private void ResetBossPosition()
        {
            _deps.EntityTransform.position = _originalPosition;
        }

        /// <summary>
        /// Completes the attack by resetting state and invoking the callback.
        /// </summary>
        private void CompleteAttack()
        {
            _isAttacking = false;
            _onAttackFinished?.Invoke();
            _onAttackFinished = null;
        }

        /// <summary>
        /// Unsubscribes from game events.
        /// </summary>
        private void UnsubscribeFromGameEvents()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
        }
    }
} 