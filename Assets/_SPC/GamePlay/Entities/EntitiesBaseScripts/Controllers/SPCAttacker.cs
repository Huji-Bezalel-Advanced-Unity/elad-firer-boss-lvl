using System.Collections.Generic;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;
namespace _SPC.GamePlay.Entities
{
    public abstract class SPCAttacker
    {
        public static Dictionary<WeaponType,int> damage = new Dictionary<WeaponType, int>
        {
            {WeaponType.PlayerBullet, 10},
            {WeaponType.BossBullet, 10},
            { WeaponType.BossBigBullet ,30},
            { WeaponType.EnemyBody ,20},
            { WeaponType.DestroyerBullet, 7},
            {WeaponType.Laser,8}
        };
        protected Transform MainTarget;
        protected Transform EntityTransform;
        protected Dictionary<WeaponType, BulletMonoPool> ProjectilePools;
        protected List<Transform> TargetTransforms;
        protected GameLogger Logger;
        protected MonoBehaviour AttackerMono;

        protected SPCAttacker(AttackerDependencies deps)
        {
            MainTarget = deps.MainTarget;
            EntityTransform = deps.EntityTransform;
            ProjectilePools = deps.ProjectilePools;
            TargetTransforms = deps.TargetTransforms;
            Logger = deps.Logger;
            AttackerMono = deps.AttackerMono;
            if(!TargetTransforms.Contains(MainTarget)) TargetTransforms.Add(MainTarget);
        }

        public abstract void Attack();
        public abstract void CleanUp();
    }
    
    
    public struct AttackerDependencies
    {
        public Transform MainTarget;
        public Transform EntityTransform;
        public Dictionary<WeaponType, BulletMonoPool> ProjectilePools;
        public List<Transform> TargetTransforms;
        public GameLogger Logger;
        public MonoBehaviour AttackerMono;
    }
    
}