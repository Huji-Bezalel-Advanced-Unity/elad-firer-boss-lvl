using System;
using _SPC.Core.Scripts.Abstracts;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    
    public class TrackingBullet: Bullet
    {
        [SerializeField] private GameObject shootEffect;
        private float _smoothFactor;
        private float _turnSpeed;

        public override void Activate(BulletInitData data)
        {
            base.Activate(data);
            Instantiate(shootEffect, transform.position - new Vector3(0, 0, 5), Quaternion.identity, transform);
            // Logs should be activated no matter what the logger state is
            if (data.smoothFactor == null)
            {
                Debug.LogException(new Exception("Smooth Factor is null For TrackingBullet")); 
                return;
            }
            if (data.turnSpeed == null)
            {
                Debug.LogException(new Exception("turnSpeed is null For TrackingBullet"));
                return;
            }
            // Cast to float here is for compiler, we cant get her in case values are null
            _turnSpeed = (float)data.turnSpeed;
            _smoothFactor = (float)data.smoothFactor; 
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            bulletLogger.Log("Triggered!");
        }

        private void FixedUpdate()
        {
            Vector2 toTarget = (Vector2)_target.position - (Vector2)transform.position;

            Vector2 desiredDirection = toTarget.normalized;

            float smoothFactor = _smoothFactor;
            _currentDirection = Vector3.Slerp(_currentDirection, desiredDirection, smoothFactor).normalized;

            float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
            rb2D.MoveRotation(angle);
            rb2D.linearVelocity = _currentDirection * _speed;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(((Vector2)_target.position - (Vector2)transform.position).normalized));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_currentDirection);
        }


        
    }
}