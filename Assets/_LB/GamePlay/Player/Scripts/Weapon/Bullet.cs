using System;
using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Generics;
using _LB.GamePlay.Player.Scripts.Controllers;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Weapon
{
    public class Bullet: LBBaseProjectile
    {
        
        private PlayerAttacker _attacker;
        [SerializeField] private GameObject explosionPrefab;
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
            var target = other.GetComponentInParent<LBBaseEntity>();
            if (target != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity,_explosionsFather);
                target.GotHit(10);
            }
            transform.position = new Vector2(-100, -100);
            _attacker.ReturnToPool(this);
        }
        
    }
}