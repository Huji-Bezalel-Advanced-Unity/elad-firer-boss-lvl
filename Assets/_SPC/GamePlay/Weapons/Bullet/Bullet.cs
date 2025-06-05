using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class Bullet: SPCBaseMono, IPoolable
    {
        [Header("Bullet")]
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected GameLogger bulletLogger;
        protected BulletMonoPool _pool;
        protected bool _active;
        protected WeaponType _weaponType;


        public virtual void Activate(WeaponType weaponType ,Vector2 target, Vector2 startPosition, float speed,float buffer, BulletMonoPool pool)
        {
            bulletLogger?.Log("Bullet activated");
            _weaponType = weaponType;
            _active = true;
            _pool = pool;
            Vector2 direction = (target - startPosition).normalized;
            Vector2 spawnPosition = startPosition + direction * buffer;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * speed;
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active) return;
            bulletLogger?.Log("Triggered by: " + other.name);
            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                target.GotHit(transform.position,_weaponType);
                bulletLogger?.Log("Bullet Hit: " + target);
            }
            transform.position = new Vector2(-100, -100);
            _pool.Return(this);
        }
        


        public virtual void Reset()
        {
            _active = false;
        }
        
        
    }
}