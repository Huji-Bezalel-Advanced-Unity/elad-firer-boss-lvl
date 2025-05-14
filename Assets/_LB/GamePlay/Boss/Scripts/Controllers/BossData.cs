using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossData: LBData
    {
        public BossData(Collider2D collider, LBStats stats) : base(collider, stats)
        {
        }

        
    }
}