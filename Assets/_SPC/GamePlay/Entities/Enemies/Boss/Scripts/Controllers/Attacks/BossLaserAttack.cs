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
    public struct BossLaserAttackDependencies
    {
        public Transform EntityTransform;
        public BoxCollider2D ArenaCollider;
        public GameObject LaserPrefab;
        public GameLogger Logger;
        public MonoBehaviour AttackerMono;
        public Transform LaserTransform;
    }

    public class BossLaserAttack : SPCAttack
    {
        private readonly BossStats _stats;
        private readonly BossLaserAttackDependencies _deps;
        private bool _isAttacking = false;
        private bool _isPaused = false;
        private Action _onAttackFinished;
        private Coroutine _laserCoroutine;

        public BossLaserAttack(BossStats stats, BossLaserAttackDependencies deps)
        {
            _stats = stats;
            _deps = deps;
            
            GameEvents.OnGamePaused += () => _isPaused = true;
            GameEvents.OnGameResumed += () => _isPaused = false;
        }

        public override bool Attack(Action onFinished = null)
        {
            if (_isAttacking) return false;
            
            _isAttacking = true;
            _onAttackFinished = onFinished;
            
            float[] cornerAngles = { 0f, 90f, 180f, 270f };
            float startAngle = cornerAngles[Random.Range(0, cornerAngles.Length)];
            float secondAngle = (startAngle + 180f) % 360f;

            Vector2 dirA = GetMoveDirection(startAngle);
            Vector2 dirB = -dirA;

            _laserCoroutine = _deps.AttackerMono.StartCoroutine(DualLaserAttack(startAngle, dirA, secondAngle, dirB));
            return true;
        }

        private IEnumerator DualLaserAttack(float angleA, Vector2 dirA, float angleB, Vector2 dirB)
        {
            bool finishedA = false, finishedB = false;
            _deps.AttackerMono.StartCoroutine(LaserRoutine(angleA, dirA, () => finishedA = true));
            _deps.AttackerMono.StartCoroutine(LaserRoutine(angleB, dirB, () => finishedB = true));
            
            while (!finishedA || !finishedB) yield return null;
            _isAttacking = false;
            _onAttackFinished?.Invoke();
            _onAttackFinished = null;
        }

        private IEnumerator LaserRoutine(float startAngle, Vector2 moveDirection, Action onLaserFinished)
        {
            // Calculate boss center
            Vector2 bossCenter = _deps.EntityTransform.position;
            Vector2 endPosition = CalculateEndPosition(startAngle, _deps.ArenaCollider);

            // Create from and to game objects
            GameObject fromGo = new GameObject("LaserFrom");
            GameObject toGo = new GameObject("LaserTo");
            fromGo.transform.position = bossCenter;
            toGo.transform.position = bossCenter;

            // Create laser
            var laserContainer = Laser.CreateLaser(_deps.LaserPrefab, _deps.LaserTransform, fromGo, toGo);

            // Stretch phase
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

            // Store the laser vector (offset from fromGo to toGo)
            Vector2 laserVector = endPosition - bossCenter;

            // Move phase
            float moveSpeed = _stats.laserMoveSpeed;
            while (_deps.ArenaCollider.OverlapPoint(laserContainer.FromT.position) && _deps.ArenaCollider.OverlapPoint(laserContainer.ToT.position))
            {
                if (_isPaused) { yield return null; continue; }
                Vector2 delta = moveDirection.normalized * moveSpeed * Time.deltaTime;
                laserContainer.FromT.position += (Vector3)delta;
                laserContainer.ToT.position = laserContainer.FromT.position + (Vector3)laserVector;
                laserContainer.Laser.UpdateBoxCollider();
                yield return null;
            }

            // Cleanup
            Object.Destroy(fromGo);
            Object.Destroy(toGo);
            Object.Destroy(laserContainer.Laser.gameObject);
            onLaserFinished?.Invoke();
        }

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

        public void Cleanup()
        {
            GameEvents.OnGamePaused -= () => _isPaused = true;
            GameEvents.OnGameResumed -= () => _isPaused = false;
            
            if (_laserCoroutine != null)
            {
                _deps.AttackerMono.StopCoroutine(_laserCoroutine);
            }
        }
    }
} 