using _SPC.Core.BaseScripts.Generics.MonoPool;
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
    public class Bullet: SPCBaseWeapon, IPoolable
    {
        [Header("Bullet")]
        [SerializeField] protected Rigidbody2D rb2D;
        protected BulletMonoPool _pool;
        protected bool _active;
        protected Vector2 _currentDirection;
        protected Transform _target;
        protected float _speed;
        private Vector2 _savedVelocity;
        

        protected override void OnGamePaused()
        {
            base.OnGamePaused();
            if (rb2D != null && rb2D.linearVelocity != Vector2.zero)
            {
                _savedVelocity = rb2D.linearVelocity;
                rb2D.linearVelocity = Vector2.zero;
            }
        }

        protected override void OnGameResumed()
        {
            base.OnGameResumed();
            if (rb2D != null)
            {
                rb2D.linearVelocity = _savedVelocity;
            }
        }

        public virtual void Activate(BulletInitData data)
        {
            weaponLogger?.Log("Bullet activated");
            _hitTransform = transform;
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
        

        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active || _isPaused) return;
            base.OnTriggerEnter2D(other);
            weaponLogger?.Log("Triggered by: " + other.name);
            transform.position = new Vector2(-100, -100);
            weaponLogger?.Log("Bullet Return");
            _pool.Return(this);
        }
        


        public virtual void Reset()
        {
            _target = null;
            _hitSuccess = false;
            _active = false;
        }
        
    }
}