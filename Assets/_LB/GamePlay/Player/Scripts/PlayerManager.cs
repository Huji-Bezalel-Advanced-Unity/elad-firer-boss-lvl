using _LB.Core.Scripts.AbstractsMono;
using _LB.GamePlay.Boss.Scripts.States;
using _LB.GamePlay.Player.Scripts.Controllers;
using _LB.GamePlay.Player.Scripts.States;
using _LB.GamePlay.Player.Scripts.Weapon;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts
{
    public sealed class PlayerManager: LBBaseEntity
    {
        [SerializeField] private BulletMonoPool projectilePool;
        void Start()
        {
            Movement = new PlayerMovement(rb2D,Stats);
            Data = new PlayerData(entityCollider,Stats);
            Attacker = new PlayerAttacker(projectilePool, Stats,targetTransform,transform, Data);
            Animator = new PlayerAnimator(animator,Stats);
            StateFactory = new PlayerStatesFactory(Animator, Data,Movement,Attacker,Stats);
            Context = new PlayerContext(Animator, Movement, Data,StateFactory);
        }
        
        protected override void Update()
        {
            base.Update();
            
        }

        
        
        
    }
}