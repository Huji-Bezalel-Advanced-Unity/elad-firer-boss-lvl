using _LB.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Player.Scripts.Controllers;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class Bullet: LBBaseMono, IPoolable
    {
        
        private PlayerAttacker _attacker;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private Rigidbody2D rb2D;
        private Transform _explosionsFather;

        public void Start()
        {
            _explosionsFather = GameObject.Find("Explosions")?.transform;
        }

        public void Activate(Vector2 target, Vector2 startPosition, float speed,float buffer, PlayerAttacker attacker)
        {
            _attacker = attacker;
            Vector2 direction = (target - startPosition).normalized;
            Vector2 spawnPosition = startPosition + direction * buffer;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            transform.position = spawnPosition;
            rb2D.linearVelocity = direction * speed;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("Triggered by: " + other.name);
            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity,_explosionsFather);
            }
            transform.position = new Vector2(-100, -100);
            _attacker.ReturnToPool(this);
        }


        public void Reset()
        {
            
        }
    }
}