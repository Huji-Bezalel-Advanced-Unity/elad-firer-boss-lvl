using _LB.Core.Scripts.AbstractsMono;
using _LB.GamePlay.Boss.Scripts.Controllers;
using _LB.GamePlay.Boss.Scripts.States;


namespace _LB.GamePlay.Boss.Scripts
{
    public class BossManager: LBBaseEntity
    {
        void Start()
        {
            Movement = new BossMovement(rb2D,Stats);
            Data = new BossData(entityCollider,Stats);
            Attacker = new BossAttacker(Stats,targetTransform,transform,Data);
            Animator = new BossAnimator(animator,Stats);
            StateFactory = new BossStatesFactory(Animator, Data,Movement,Attacker,Stats);
            Context = new BossContext(Animator, Movement, Data,StateFactory);
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}