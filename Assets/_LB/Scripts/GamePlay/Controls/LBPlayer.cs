using _LB.Scripts.Core.BaseMono;
using UnityEngine;

namespace _LB.Scripts.GamePlay.Controls
{
    public class LBPlayer : LBBaseMono
    {
        private Collider _lightCollider;
        private 
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            CheckForLightCollider();
        }

        private void CheckForLightCollider()
        {
            if (_lightCollider == null) return;
            
        }
    }
}
