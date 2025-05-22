using UnityEngine;

namespace _LB.Core.Scripts.Interfaces
{
    public interface IHitable
    {
        public void GotHit(Vector3 projectileTransform);
    }
}