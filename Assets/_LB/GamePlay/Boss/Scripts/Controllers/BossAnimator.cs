using _LB.Core.Scripts.Abstracts;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossAnimator: LBAnimator
    {
        public BossAnimator(Animator animator, ILBStats stats) : base(animator, stats)
        {
        }
    }
}