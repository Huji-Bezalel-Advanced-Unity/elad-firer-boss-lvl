using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBData
    { 
        protected Collider2D MainCollider;
        protected ILBStats Stats;
        protected LBData(Collider2D collider, ILBStats stats)
        {
            MainCollider = collider;
            Stats = stats;
        } 
    }
}