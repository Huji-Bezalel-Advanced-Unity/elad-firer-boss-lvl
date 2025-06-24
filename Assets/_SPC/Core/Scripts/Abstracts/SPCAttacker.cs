using System.Collections.Generic;
using _SPC.Core.Scripts.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.Core.Scripts.Abstracts
{
    public abstract class SPCAttacker
    {
        public static Dictionary<WeaponType,int> damage = new Dictionary<WeaponType, int>
        {
            {WeaponType.PlayerBullet, 10},
            {WeaponType.BossBullet, 10},
            { WeaponType.BossBigBullet ,30},
            { WeaponType.EnemyBody ,20},
            { WeaponType.DestroyerBullet, 7}
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
    
    public enum WeaponType
    {
        BossBullet,
        BossBigBullet,
        PlayerBullet,
        EnemyBody,
        DestroyerBullet
    }
}