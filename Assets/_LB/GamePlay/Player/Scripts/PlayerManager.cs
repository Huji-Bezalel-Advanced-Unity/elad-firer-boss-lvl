using _LB.Core.Scripts.BaseMono;
using _LB.GamePlay.Player.Scripts.Controllers;
using _LB.GamePlay.Player.Scripts.States;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts
{
    public class PlayerManager: LBBaseEntity
    {
        
        void Start()
        {
            Movement = new PlayerMovement(rb2D);
            Data = new PlayerData(entityCollider);
            Animator = new PlayerAnimator(animator);
            Context = new PlayerContext(Animator, Movement, Data);

        }
    }
}