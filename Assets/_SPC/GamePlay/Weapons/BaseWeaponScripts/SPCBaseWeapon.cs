using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Managers;
using _SPC.GamePlay.Entities;
using _SPC.GamePlay.Utils;
using UnityEngine;

namespace _SPC.GamePlay.Weapons
{
    public class SPCBaseWeapon: SPCBaseMono
    {
        
        [Header("Weapon Fields")]
        [SerializeField] protected GameLogger weaponLogger;
        [SerializeField] protected WeaponType _weaponType;
        protected bool _hitSuccess;
        protected bool _isPaused = false;
        protected IHitable _target;
        protected Transform _hitTransform;


        protected virtual void OnEnable()
        {
            GameEvents.OnGamePaused += OnGamePaused;
            GameEvents.OnGameResumed += OnGameResumed;
        }

        protected virtual void OnDisable()
        {
            GameEvents.OnGamePaused -= OnGamePaused;
            GameEvents.OnGameResumed -= OnGameResumed;
        }
        
        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            var target = other.GetComponentInParent<IHitable>();
            if (target != null)
            {
                _target = target;
                target.GotHit(_hitTransform.position,_weaponType);
                weaponLogger?.Log("Bullet Hit: " + target);
                _hitSuccess = true;
            }
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
            if (_target != null)
            {
                _target = null;
                _hitSuccess = false;
            }
        }


        protected virtual void OnGameResumed()
        {
           _isPaused = false;
        }

        protected virtual void OnGamePaused()
        {
            _isPaused = true;
        }
    }
    
    public enum WeaponType
    {
        BossBullet,
        BossBigBullet,
        PlayerBullet,
        EnemyBody,
        DestroyerBullet,
        Laser
    }
}