
using System.Collections.Generic;
using _SPC.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
using Type = _SPC.Core.Scripts.Interfaces.Type;

namespace _SPC.GamePlay.Enemies.LBBaseEnemy
{
    public abstract class LBBaseEnemy: SPCBaseMono, IHitable
    {
        [Header("Attacker")]
        [SerializeField] protected GameObject explosionPrefab;
        [SerializeField] protected Transform _explosionsFather;
        [SerializeField] protected BulletMonoPool bulletPool;
        [SerializeField] protected List<Transform> transformTargets = new List<Transform>();
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected GameLogger  enemyLogger;
        
        public void GotHit(Vector3 projectileTransform)
        {
            Instantiate(explosionPrefab, projectileTransform, Quaternion.identity,_explosionsFather);
        }

        public Type GetTypeOfEntity()
        {
            return Type.Enemy;
        }
    }

   
}