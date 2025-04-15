using _LB.Core.Scripts.Abstracts;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossMovement: LBMovement
    {
        public BossMovement(Rigidbody2D rb, ILBStats stats) : base(rb, stats)
        {
        }

        public override void UpdateMovement()
        {
            throw new System.NotImplementedException();
        }
    }
}