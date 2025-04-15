using _LB.Core.Scripts.AbstractsMono;
using _LB.Core.Scripts.Generics;
using UnityEngine;

namespace _LB.GamePlay.Player.Scripts.Weapon
{
    public class Bullet: LBBaseProjectile
    {
        public override void Activate(Vector2 target, Vector2 startPosition, float speed, float buffer, LBMonoPool<LBBaseProjectile> pool)
        {
            base.Activate(target, startPosition, speed, buffer, pool);
        }
    }
}