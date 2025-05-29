using UnityEngine;

namespace _SPC.Core.Scripts.Interfaces
{
    public interface IHitable
    {
        public void GotHit(Vector3 projectileTransform);
        public Type GetTypeOfEntity();
    }
    
    public enum Type
    {
        Player,
        Enemy
    }
}