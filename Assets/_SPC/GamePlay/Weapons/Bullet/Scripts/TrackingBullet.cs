using System;
using UnityEngine;

namespace _SPC.GamePlay.Weapons.Bullet
{
    /// <summary>
    /// Bullet that tracks and follows a target with smooth turning behavior.
    /// Continuously updates direction to follow the target's position.
    /// </summary>
    public class TrackingBullet : Bullet
    {
        [Header("Tracking Configuration")]
        [Tooltip("Smoothing factor for direction interpolation (0-1). Higher values = faster turning.")]
        private float _smoothFactor;
        
        [Tooltip("Maximum turn speed in degrees per second.")]
        private float _turnSpeed;

        /// <summary>
        /// Activates the tracking bullet with additional tracking parameters.
        /// Validates that required tracking parameters are provided.
        /// </summary>
        /// <param name="data">Initialization data containing bullet and tracking parameters.</param>
        public override void Activate(BulletInitData data)
        {
            base.Activate(data);
            
            // Validate tracking parameters
            if (data.smoothFactor == null)
            {
                Debug.LogException(new Exception("SmoothFactor is null for TrackingBullet"));
                return;
            }
            
            if (data.turnSpeed == null)
            {
                Debug.LogException(new Exception("TurnSpeed is null for TrackingBullet"));
                return;
            }
            
            // Cast to float here is for compiler, we can't get here if values are null
            _turnSpeed = (float)data.turnSpeed;
            _smoothFactor = (float)data.smoothFactor;
        }

        /// <summary>
        /// Updates bullet direction every physics frame to track the target.
        /// Uses smooth interpolation to gradually turn towards the target.
        /// </summary>
        private void FixedUpdate()
        {   
            if (_isPaused || _target == null) return;
            
            Vector2 toTarget = (Vector2)_target.position - (Vector2)transform.position;
            Vector2 desiredDirection = toTarget.normalized;

            
            _currentDirection = Vector3.Slerp(_currentDirection, desiredDirection, _smoothFactor).normalized;

            
            float angle = Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg;
            rb2D.MoveRotation(angle);
            rb2D.linearVelocity = _currentDirection * _speed;
        }
        
        /// <summary>
        /// Draws debug gizmos to visualize tracking behavior in the Scene view.
        /// Blue line shows direction to target, red line shows current movement direction.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_target == null) return;

            
            Gizmos.color = Color.blue;
            Vector2 toTarget = ((Vector2)_target.position - (Vector2)transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)toTarget);

           
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_currentDirection);
        }
    }
}