using System.Collections.Generic;
using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;

using _SPC.GamePlay.Weapons.Bullet;

using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies
{
    public abstract class SpcBaseEnemy: SPCBaseMono, IHitable
    {
        [Header("Attacker")]
        [SerializeField] protected GameObject explosionPrefab;
        [SerializeField] protected Transform _explosionsFather;
        [SerializeField] protected BulletMonoPool bulletPool;
        [SerializeField] protected List<Transform> transformTargets = new List<Transform>();
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected GameLogger enemyLogger;
        [SerializeField] protected Collider2D collider;
        
        [Header("UI")] 
        [SerializeField] protected HealthBarUI healthBarUI;
        
        protected SPCHealth _health;
        protected SPCAttacker _attacker;

        private void Start()
        {
            GameEvents.EnemyAdded(transform);
        }

        private void OnDestroy()
        {
            GameEvents.EnemyRemoved(transform);
        }

        public virtual void GotHit(Vector3 projectileTransform, WeaponType weaponType)
        {
            _health.ReduceLife(SPCAttacker.damage[weaponType]);
            Instantiate(explosionPrefab, projectileTransform, Quaternion.identity,_explosionsFather);
            if (weaponType == WeaponType.PlayerBullet)
            {
                GameEvents.PlayerHit(projectileTransform);
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

        protected virtual void Init(Transform mainTarget, List<Transform> targets, GameObject explosionPrefab, Transform explosionsFather, BulletMonoPool pool)
        {
            targetTransform = mainTarget;
            transformTargets = targets;
            this.explosionPrefab = explosionPrefab;
            _explosionsFather = explosionsFather;
            bulletPool = pool;
        }
    }

   
}