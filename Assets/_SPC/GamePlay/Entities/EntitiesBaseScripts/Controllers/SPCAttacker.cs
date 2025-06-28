using System.Collections.Generic;
using _SPC.GamePlay.Utils;
using _SPC.GamePlay.Weapons;
using _SPC.GamePlay.Weapons.Bullet;
using UnityEngine;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Base class for all attacker entities, providing common functionality for weapon damage and projectile management.
    /// </summary>
    public abstract class SPCAttacker
    {
        /// <summary>
        /// Dictionary mapping weapon types to their damage values.
        /// </summary>
        public static Dictionary<WeaponType, int> damage = new Dictionary<WeaponType, int>
        {
            {WeaponType.PlayerBullet, 10},
            {WeaponType.BossBullet, 10},
            {WeaponType.BossBigBullet, 30},
            {WeaponType.EnemyBody, 20},
            {WeaponType.DestroyerBullet, 7},
            {WeaponType.Laser, 8}
        };

        protected Transform MainTarget;
        protected Transform EntityTransform;
        protected Dictionary<WeaponType, BulletMonoPool> ProjectilePools;
        protected List<Transform> TargetTransforms;
        protected GameLogger Logger;
        protected MonoBehaviour AttackerMono;

        /// <summary>
        /// Initializes the attacker with dependencies and ensures main target is in target list.
        /// </summary>
        /// <param name="deps">Dependencies required for the attacker.</param>
        protected SPCAttacker(AttackerDependencies deps)
        {
            InitializeDependencies(deps);
            EnsureMainTargetInList();
        }

        /// <summary>
        /// Abstract method for executing attacks. Must be implemented by derived classes.
        /// </summary>
        public abstract void Attack();

        /// <summary>
        /// Abstract method for cleaning up resources. Must be implemented by derived classes.
        /// </summary>
        public abstract void CleanUp();

        /// <summary>
        /// Initializes all dependencies from the provided struct.
        /// </summary>
        /// <param name="deps">Dependencies to initialize from.</param>
        private void InitializeDependencies(AttackerDependencies deps)
        {
            MainTarget = deps.MainTarget;
            EntityTransform = deps.EntityTransform;
            ProjectilePools = deps.ProjectilePools;
            TargetTransforms = deps.TargetTransforms;
            Logger = deps.Logger;
            AttackerMono = deps.AttackerMono;
        }

        /// <summary>
        /// Ensures the main target is included in the target transforms list.
        /// </summary>
        private void EnsureMainTargetInList()
        {
            if (!TargetTransforms.Contains(MainTarget))
            {
                TargetTransforms.Add(MainTarget);
            }
        }
    }

    /// <summary>
    /// Holds dependencies required by the SPCAttacker for initialization.
    /// </summary>
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