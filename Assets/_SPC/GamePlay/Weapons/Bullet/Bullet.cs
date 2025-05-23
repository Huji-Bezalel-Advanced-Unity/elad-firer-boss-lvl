using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.GamePlay.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class Bullet: SPCBaseMono, IPoolable
    {
        
        [SerializeField] private Rigidbody2D rb2D;
        [SerializeField] private GameLogger bulletLogger;
        private BulletMonoPool _pool;
        private bool _active;


        public void Activate(Vector2 target, Vector2 startPosition, float speed,float buffer, BulletMonoPool pool)
        {
            bulletLogger?.Log("Bullet activated");
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
            }
            transform.position = new Vector2(-100, -100);
            _pool.Return(this);
        }
        


        public void Reset()
        {
            
        }
    }
}