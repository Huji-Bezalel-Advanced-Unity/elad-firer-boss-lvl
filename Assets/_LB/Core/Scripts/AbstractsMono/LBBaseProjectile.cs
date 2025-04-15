using _LB.Core.Scripts.Generics;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsMono
{
    public abstract class LBBaseProjectile : LBBaseMono, ILBPoolable
    {
        [SerializeField] protected Rigidbody2D rb2D;
        
        
        protected LBMonoPool<LBBaseProjectile> ProjectilePool;
        
        public void Reset()
        {
            rb2D.linearVelocity = Vector2.zero;
        }

        public virtual void Activate(Vector2 target, Vector2 startPosition, float speed,float buffer,LBMonoPool<LBBaseProjectile> pool)
        {
            ProjectilePool = pool;
            Vector2 direction = (target - startPosition).normalized;
            Vector2 spawnPosition = startPosition + direction * buffer;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * speed;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            ProjectilePool.Return(this);
        }
    }
}