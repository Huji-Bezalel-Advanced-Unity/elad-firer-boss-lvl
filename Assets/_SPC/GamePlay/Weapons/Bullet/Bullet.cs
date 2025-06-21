using _SPC.Core.Scripts.Abstracts;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    
    public struct BulletInitData
    {
        public WeaponType weaponType;
        public Transform target;
        public Vector2 startPosition;
        public float speed;
        public float buffer;
        public float? smoothFactor; // Nullable — only used by tracking bullets
        public float? turnSpeed;
        public BulletMonoPool pool;

       public BulletInitData(WeaponType weaponType, Transform target, Vector2 startPosition,
                          float speed, float buffer, BulletMonoPool pool, float? smoothFactor = null, float? turnSpeed = null)
    {
        this.weaponType = weaponType;
        this.target = target;
        this.startPosition = startPosition;
        this.speed = speed;
        this.buffer = buffer;
        this.pool = pool;
        this.smoothFactor = smoothFactor;
        this.turnSpeed = turnSpeed;
    }
    }
    public class Bullet: SPCBaseMono, IPoolable
    {
        [Header("Bullet")]
        [SerializeField] protected Rigidbody2D rb2D;
        [SerializeField] protected GameLogger bulletLogger;
        protected BulletMonoPool _pool;
        protected bool _active;
        protected WeaponType _weaponType;
        protected Vector2 _currentDirection;
        protected Transform _target;
        protected float _speed;


        public virtual void Activate(BulletInitData data)
        {
            bulletLogger?.Log("Bullet activated");
            _weaponType = data.weaponType;
            _active = true;
            _pool = data.pool;
            _target = data.target;
            _speed = data.speed;

            Vector2 direction = ((Vector2)data.target.position - data.startPosition).normalized;
            _currentDirection = direction;
            Vector2 spawnPosition = data.startPosition + direction * data.buffer;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * data.speed;
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