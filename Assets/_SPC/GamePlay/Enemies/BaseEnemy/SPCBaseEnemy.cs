using System.Collections.Generic;
using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Enemies.BaseEnemy
{
    public abstract class SPCBaseEnemy: SPCBaseMono, IHitable
    {
        [Header("Attacker")]
        [SerializeField] protected GameObject explosionPrefab;
        [SerializeField] protected Transform _explosionsFather;
        [SerializeField] protected BulletMonoPool bulletPool;
        [SerializeField] protected List<Transform> transformTargets = new List<Transform>();
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected GameLogger  enemyLogger;
        [SerializeField] protected Collider2D collider;
        
        protected SPCAttacker _attacker;

        void Start()
        {
            GameEvents.EnemyAdded(transform);
        }
        public virtual void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            Instantiate(explosionPrefab, projectileTransform, Quaternion.identity,_explosionsFather);
            if (weaponType == WeaponType.PlayerBullet)
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

        public virtual void Init(Transform mainTarget, List<Transform> targets, GameObject explosionPrefab, Transform explosionsFather, BulletMonoPool pool)
        {
            targetTransform = mainTarget;
            transformTargets = targets;
            this.explosionPrefab = explosionPrefab;
            _explosionsFather = explosionsFather;
            bulletPool = pool;
        }
    }

   
}