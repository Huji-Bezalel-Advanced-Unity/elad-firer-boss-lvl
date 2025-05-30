using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class Bullet: SPCBaseMono, IPoolable
    {
        
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private GameLogger bulletLogger;
        private BulletMonoPool _pool;
        private bool _active;
        private Type _shooterType;


        public void Activate(Type shooterType ,Vector2 target, Vector2 startPosition, float speed,float buffer, BulletMonoPool pool)
        {
            bulletLogger?.Log("Bullet activated");
            _shooterType = shooterType;
            _active = true;
            _pool = pool;
            Vector2 direction = (target - startPosition).normalized;
            Vector2 spawnPosition = startPosition + direction * buffer;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * speed;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!_active) return;
            bulletLogger?.Log("Triggered by: " + other.name);
            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                target.GotHit(transform.position);
                bulletLogger?.Log("Bullet Hit: " + target);
                if (_shooterType == Type.Player && target.GetTypeOfEntity() == Type.Enemy )
                {
                    GameEvents.PlayerHit();
                }
                if (_shooterType == Type.Enemy && target.GetTypeOfEntity() == Type.Player )
                {
                    GameEvents.EnemyHit(10);
                }
            }
            transform.position = new Vector2(-100, -100);
            _pool.Return(this);
        }
        


        public void Reset()
        {
            _active = false;
        }
        
    }
}