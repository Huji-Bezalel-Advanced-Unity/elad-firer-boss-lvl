using System;
using _LB.Core.Scripts.Interfaces;
using _SPC.Core.Scripts.LBBaseMono;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _SPC.GamePlay.Enemies.LBBaseEnemy
{
    public abstract class LBBaseEnemy: LBBaseMono, IHitable
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private Transform _explosionsFather;

        public void GotHit(Vector3 projectileTransform)
        {
            Instantiate(explosionPrefab, projectileTransform, Quaternion.identity,_explosionsFather);
        }
    }

   
}