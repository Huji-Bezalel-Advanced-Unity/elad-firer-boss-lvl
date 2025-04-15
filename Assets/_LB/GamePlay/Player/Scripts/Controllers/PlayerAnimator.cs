using _LB.Core.Scripts.Abstracts;
using _LB.Core.Scripts.Interfaces;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Controllers
{
    public class PlayerAnimator: LBAnimator
    {
        public PlayerAnimator(Animator animator, ILBStats stats) : base(animator, stats)
        {
        }
    }
}