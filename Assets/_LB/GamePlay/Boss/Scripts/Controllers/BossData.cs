using _LB.Core.Scripts.Abstracts;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossData: LBData
    {
        public BossData(Collider2D collider, ILBStats stats) : base(collider, stats)
        {
        }
    }
}