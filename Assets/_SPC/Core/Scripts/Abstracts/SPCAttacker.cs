using System.Collections.Generic;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.Core.Scripts.Abstracts
{
    public abstract class SPCAttacker
    {
        protected Transform MainTarget;
        protected Transform EntityTransform;
        protected Dictionary<BulletType, BulletMonoPool> ProjectilePools;
        protected List<Transform> TargetTransforms;
        protected GameLogger Logger;

        protected SPCAttacker(AttackerDependencies deps)
        {
            MainTarget = deps.MainTarget;
            EntityTransform = deps.EntityTransform;
            ProjectilePools = deps.ProjectilePools;
            TargetTransforms = deps.TargetTransforms;
            Logger = deps.Logger;
            TargetTransforms.Add(MainTarget);
        }
    }
    
    
    public struct AttackerDependencies
    {
        public Transform MainTarget;
        public Transform EntityTransform;
        public Dictionary<BulletType, BulletMonoPool> ProjectilePools;
        public List<Transform> TargetTransforms;
        public GameLogger Logger;
    }
    
    public enum BulletType
    {
        EnemyBullet,
        PlayerBullet
    }
}