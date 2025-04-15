using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public sealed class PlayerMovement : LBMovement
    {
        private const float Acceleration = 100f;
        private const float MovementThreshold = 0.05f;

        public PlayerMovement(Rigidbody2D rb, LBStats stats) : base(rb, stats)
        {
            // Set drag to simulate natural slow-down when no input is given
           

            // Optional: Set mass for better control over force-based movement
            Rigidbody.mass = 1f;
        }

        public override void UpdateMovement()
        {
            // Get smooth analog input (-1 to 1)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector2 input = new Vector2(horizontal, vertical);

            if (input.sqrMagnitude < MovementThreshold)
                return;

            Direction = input.normalized;

            // Apply continuous force based on direction and acceleration
            Rigidbody.AddForce(Direction * Acceleration * Time.deltaTime, ForceMode2D.Force);
        }
    }
}