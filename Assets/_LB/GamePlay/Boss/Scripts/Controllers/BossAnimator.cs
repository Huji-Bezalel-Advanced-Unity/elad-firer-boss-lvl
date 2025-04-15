using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts.Controllers
{
    public class BossAnimator: LBAnimator
    {
        public BossAnimator(Animator animator, LBStats stats) : base(animator, stats)
        {
        }
    }
}