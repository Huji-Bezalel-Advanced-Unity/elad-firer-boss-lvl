using _LB.Core.Scripts.Abstracts;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossMovement: LBMovement
    {
        public BossMovement(Rigidbody2D rb) : base(rb)
        {
        }

        public override void UpdateMovement()
        {
            throw new System.NotImplementedException();
        }
    }
}