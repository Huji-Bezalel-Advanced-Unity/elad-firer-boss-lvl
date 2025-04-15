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
            Movement = new BossMovement(rb2D,Stats);
            Data = new BossData(entityCollider,Stats);
            Animator = new BossAnimator(animator,Stats);
            StateFactory = new BossStatesFactory(Animator, Data,Movement);
            Context = new BossContext(Animator, Movement, Data,StateFactory);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}