using _SPC.Core.BaseScripts.BaseMono;
using _SPC.GamePlay.Entities;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    public class SPCBaseWeapon: SPCBaseMono
    {
        
        [Header("Weapon Fields")]
        [SerializeField] protected GameLogger weaponLogger;
        
        protected WeaponType _weaponType;
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                target.GotHit(transform.position,_weaponType);
                weaponLogger?.Log("Bullet Hit: " + target);
            }
        }
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