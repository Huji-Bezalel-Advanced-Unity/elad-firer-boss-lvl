using _LB.Core.Scripts.BaseMono;
using _LB.GamePlay.Boss.Scripts.States;
using _LB.GamePlay.Player.Scripts.Controllers;
using _LB.GamePlay.Player.Scripts.States;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts
{
    public sealed class PlayerManager: LBBaseEntity
    {
        void Start()
        {
            Movement = new PlayerMovement(rb2D,Stats);
            Data = new PlayerData(entityCollider,Stats);
            Animator = new PlayerAnimator(animator,Stats);
            StateFactory = new PlayerStatesFactory(Animator, Data,Movement);
            Context = new PlayerContext(Animator, Movement, Data,StateFactory);
        }
        
        protected override void Update()
        {
            base.Update();
            
        }

        
        
        
    }
}