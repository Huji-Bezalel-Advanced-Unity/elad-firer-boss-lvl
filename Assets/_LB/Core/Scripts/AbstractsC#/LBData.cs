using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.Core.Scripts.AbstractsC_
{
    public abstract class LBData
    { 
        protected Collider2D MainCollider;
        protected LBStats Stats;
        protected LBData(Collider2D collider, LBStats stats)
        {
            MainCollider = collider;
            Stats = stats;
        } 
        
       
    }
}