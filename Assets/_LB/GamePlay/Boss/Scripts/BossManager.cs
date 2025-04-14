using _LB.Core.Scripts.BaseMono;
using _LB.GamePlay.Boss.Scripts.Controllers;
using _LB.GamePlay.Boss.Scripts.States;
using UnityEngine;

namespace _LB.GamePlay.Boss.Scripts
{
    public class BossManager: LBBaseEntity
    {
        void Start()
        {
            Movement = new BossMovement(rb2D);
            Data = new BossData(entityCollider);
            Animator = new BossAnimator(animator);
            Context = new BossContext(Animator, Movement, Data);
        }
    }
}