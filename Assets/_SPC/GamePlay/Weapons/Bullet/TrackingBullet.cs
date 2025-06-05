using System;
using _SPC.Core.Scripts.Abstracts;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    public class TrackingBullet: Bullet
    {
        [SerializeField] private GameObject shootEffect;
        [SerializeField] private float turnSpeed = 5f; // How quickly the bullet turns toward the target
        private Vector2 _targetPosition;
        private float _speed;

        public override void Activate(WeaponType weaponType, Vector2 target, Vector2 startPosition, float speed, float buffer,
            BulletMonoPool pool)
        {
            base.Activate(weaponType, target, startPosition, speed, buffer, pool);
            Instantiate(shootEffect, transform.position  - new Vector3(0,0,5), Quaternion.identity, transform); 
            _targetPosition = target;
            _speed = speed;
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            bulletLogger.Log("Triggered!");
        }

        public void Update()
        {
            if (rb2D == null) return;
            Vector2 currentPosition = rb2D.position;
            Vector2 toTarget = (_targetPosition - currentPosition).normalized;
            Vector2 currentVelocity = rb2D.linearVelocity.normalized;
            // Slerp for smooth turning
            Vector2 newDirection = Vector2.Lerp(currentVelocity, toTarget, turnSpeed * Time.deltaTime).normalized;
            rb2D.linearVelocity = newDirection * _speed;
            // Optionally rotate the bullet to face its velocity
            float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        
        
        
    }
}