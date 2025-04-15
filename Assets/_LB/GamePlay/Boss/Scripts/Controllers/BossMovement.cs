using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossMovement: LBMovement
    {
        public BossMovement(Rigidbody2D rb, LBStats stats) : base(rb, stats)
        {
        }

        public override void UpdateMovement()
        {
            throw new System.NotImplementedException();
        }
    }
}