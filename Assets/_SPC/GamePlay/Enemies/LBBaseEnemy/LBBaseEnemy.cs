
using System;
using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.LBBaseEnemy
{
    public abstract class LBBaseEnemy: SPCBaseMono, IHitable
    {
        [Header("Attacker")]
        [SerializeField] protected GameObject explosionPrefab;
        [SerializeField] protected Transform _explosionsFather;
        [SerializeField] protected BulletMonoPool bulletPool;
        [SerializeField] protected List<Transform> transformTargets = new List<Transform>();
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected GameLogger  enemyLogger;
        
        public void GotHit(Vector3 projectileTransform, WeaponType shooterType)
        {
            Instantiate(explosionPrefab, projectileTransform, Quaternion.identity,_explosionsFather);
            if (shooterType == WeaponType.PlayerBullet)
            {
                GameEvents.PlayerHit();
            }
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            var hit = other.gameObject.GetComponentInParent<IHitable>();
            if (hit != null)
            {
                hit.GotHit(transform.position,WeaponType.EnemyBody);
                
            }
        }
    }

   
}