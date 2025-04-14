using UnityEngine;

namespace _LB.Core.Scripts.Abstracts
{
    public abstract class LBData
    { 
        protected Collider2D MainCollider;

        protected LBData(Collider2D collider)
        {
            MainCollider = collider;
        } 
    }
}