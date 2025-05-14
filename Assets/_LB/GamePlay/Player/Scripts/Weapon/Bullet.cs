using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.GamePlay.Player.Scripts.Controllers;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Weapon
{
    public class Bullet: LBBaseProjectile
    {
        
        private PlayerAttacker _attacker;
        
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
            transform.position = new Vector2(-100, -100);
            _attacker.ReturnToPool(this);
        }
        
    }
}