using _LB.Core.Scripts.AbstractsC_;
using _LB.Core.Scripts.AbstractsScriptable;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerAnimator: LBAnimator
    {
        public PlayerAnimator(Animator animator, LBStats stats) : base(animator, stats)
        {
        }
    }
}