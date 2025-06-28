using System;
using System.Collections;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossLaserAttack, including arena, laser prefab, and transforms.
    /// </summary>
    public struct BossLaserAttackDependencies
    {
        public Transform EntityTransform;
        public BoxCollider2D ArenaCollider;
        public GameObject LaserPrefab;
        public GameLogger Logger;
        public MonoBehaviour AttackerMono;
        public Transform LaserTransform;
    }

    /// <summary>
    /// Handles the boss's dual laser attack that fires from opposite corners and moves along arena edges.
    /// </summary>
    public class BossLaserAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossLaserAttackDependencies _deps;
        private bool _isAttacking = false;
        private bool _isPaused = false;
        private Action _onAttackFinished;
        private Coroutine _laserCoroutine;

        /// <summary>
        /// Initializes the BossLaserAttack with stats and dependencies.
        /// </summary>
        public BossLaserAttack(BossStats stats, BossLaserAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            SubscribeToGameEvents();
        }

        /// <summary>
        /// Executes the dual laser attack from opposite corners.
        /// </summary>
        public override bool Attack(Action onFinished = null)
        {
            if (_isAttacking) return false;
            
            _isAttacking = true;
            _onAttackFinished = onFinished;
            
            var attackDirections = CalculateAttackDirections();
            _laserCoroutine = _deps.AttackerMono.StartCoroutine(DualLaserAttack(attackDirections));
            
            return true;
        }

        /// <summary>
        /// Cleans up event subscriptions and stops any ongoing laser coroutine.
        /// </summary>
        public void Cleanup()
        {
            UnsubscribeFromGameEvents();
            StopLaserCoroutine();
        }

        /// <summary>
        /// Subscribes to game pause/resume events.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }

        /// <summary>
        /// Calculates the attack directions for the dual laser attack.
        /// </summary>
        private (float angleA, Vector2 dirA, float angleB, Vector2 dirB) CalculateAttackDirections()
        {
            float[] cornerAngles = { 0f, 90f, 180f, 270f };
            float startAngle = cornerAngles[Random.Range(0, cornerAngles.Length)];
            float secondAngle = (startAngle + 180f) % 360f;

            Vector2 dirA = GetMoveDirection(startAngle);
            Vector2 dirB = -dirA;

            return (startAngle, dirA, secondAngle, dirB);
        }

        /// <summary>
        /// Executes the dual laser attack with two lasers firing simultaneously.
        /// </summary>
        private IEnumerator DualLaserAttack((float angleA, Vector2 dirA, float angleB, Vector2 dirB) directions)
        {
            bool finishedA = false, finishedB = false;
            
            _deps.AttackerMono.StartCoroutine(LaserRoutine(directions.angleA, directions.dirA, () => finishedA = true));
            _deps.AttackerMono.StartCoroutine(LaserRoutine(directions.angleB, directions.dirB, () => finishedB = true));
            
            yield return new WaitUntil(() => finishedA && finishedB);
            
            CompleteAttack();
        }

        /// <summary>
        /// Handles the individual laser routine for stretching and moving.
        /// </summary>
        private IEnumerator LaserRoutine(float startAngle, Vector2 moveDirection, Action onLaserFinished)
        {
            Vector2 bossCenter = _deps.EntityTransform.position;
            Vector2 endPosition = CalculateEndPosition(startAngle, _deps.ArenaCollider);

            var laserContainer = CreateLaserContainer(bossCenter, endPosition);
            
            yield return StretchLaserPhase(laserContainer, bossCenter, endPosition);
            yield return MoveLaserPhase(laserContainer, moveDirection);
            
            CleanupLaser(laserContainer);
            onLaserFinished?.Invoke();
        }

        /// <summary>
        /// Creates the laser container with from/to game objects and laser component.
        /// </summary>
        private LaserContainer CreateLaserContainer(Vector2 bossCenter, Vector2 endPosition)
        {
            GameObject fromGo = new GameObject("LaserFrom");
            GameObject toGo = new GameObject("LaserTo");
            fromGo.transform.position = bossCenter;
            toGo.transform.position = bossCenter;

            var laserContainer = Laser.CreateLaser(_deps.LaserPrefab, _deps.LaserTransform, fromGo, toGo);
            return laserContainer;
        }

        /// <summary>
        /// Handles the laser stretching phase.
        /// </summary>
        private IEnumerator StretchLaserPhase(LaserContainer laserContainer, Vector2 bossCenter, Vector2 endPosition)
        {
            float stretchDuration = _stats.laserStretchTime;
            float elapsed = 0f;
            
            while (elapsed < stretchDuration)
            {
                if (_isPaused) { yield return null; continue; }
                
                float progress = elapsed / stretchDuration;
                Vector2 currentEndPos = Vector2.Lerp(bossCenter, endPosition, progress);
                laserContainer.ToT.position = currentEndPos;
                laserContainer.Laser.UpdateBoxCollider();
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            laserContainer.Laser.UpdateBoxCollider();
        }

        /// <summary>
        /// Handles the laser movement phase along the arena edge.
        /// </summary>
        private IEnumerator MoveLaserPhase(LaserContainer laserContainer, Vector2 moveDirection)
        {
            Vector2 laserVector = laserContainer.ToT.position - laserContainer.FromT.position;
            float moveSpeed = _stats.laserMoveSpeed;
            
            while (IsLaserInArena(laserContainer))
            {
                if (_isPaused) { yield return null; continue; }
                
                Vector2 delta = moveDirection.normalized * moveSpeed * Time.deltaTime;
                laserContainer.FromT.position += (Vector3)delta;
                laserContainer.ToT.position = laserContainer.FromT.position + (Vector3)laserVector;
                laserContainer.Laser.UpdateBoxCollider();
                
                yield return null;
            }
        }

        /// <summary>
        /// Checks if the laser is still within the arena bounds.
        /// </summary>
        private bool IsLaserInArena(LaserContainer laserContainer)
        {
            return _deps.ArenaCollider.OverlapPoint(laserContainer.FromT.position) && 
                   _deps.ArenaCollider.OverlapPoint(laserContainer.ToT.position);
        }

        /// <summary>
        /// Cleans up the laser game objects.
        /// </summary>
        private void CleanupLaser(LaserContainer laserContainer)
        {
            Object.Destroy(laserContainer.FromT.gameObject);
            Object.Destroy(laserContainer.ToT.gameObject);
            Object.Destroy(laserContainer.Laser.gameObject);
        }

        /// <summary>
        /// Calculates the end position for the laser based on the start angle and arena bounds.
        /// </summary>
        private Vector2 CalculateEndPosition(float angle, BoxCollider2D collider)
        {
            var bounds = collider.bounds;
            switch (angle)
            {
                case 0:   
                    return new Vector2(bounds.center.x, bounds.max.y);
                case 90:  
                    return new Vector2(bounds.max.x, bounds.center.y);
                case 180: 
                    return new Vector2(bounds.center.x, bounds.min.y);
                case 270:
                    return new Vector2(bounds.min.x, bounds.center.y);
                default:
                    return bounds.center;
            }
        }

        /// <summary>
        /// Gets the movement direction for the laser based on the start angle.
        /// </summary>
        private Vector2 GetMoveDirection(float startAngle)
        {
            if (startAngle == 0F || Mathf.Approximately(startAngle, 180f))
            {
                return Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                return Random.Range(0, 2) == 0 ? Vector2.up : Vector2.down;
            }
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
            GameEvents.OnGamePaused -= () => _isPaused = true;
            GameEvents.OnGameResumed -= () => _isPaused = false;
        }

        /// <summary>
        /// Stops the laser coroutine if it's running.
        /// </summary>
        private void StopLaserCoroutine()
        {
            if (_laserCoroutine != null)
            {
                _deps.AttackerMono.StopCoroutine(_laserCoroutine);
            }
        }
    }
} 