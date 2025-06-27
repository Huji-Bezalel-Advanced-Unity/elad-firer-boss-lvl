using System;
using _SPC.Core.BaseScripts.BaseMono;
using _SPC.Core.BaseScripts.Generics.MonoPool;
using _SPC.Core.BaseScripts.Managers;
using UnityEngine;


namespace _SPC.Core.Audio
{
    public class SoundObject : SPCBaseMono, IPoolable
    {
        [SerializeField] public AudioSource audioSource;
        private SoundPool _soundPool;
        private bool _active;
        private Action _callback;
        private bool _isPauseble;

        public void OnEnable()
        {
            GameEvents.OnGameResumed += OnGameResume;
            GameEvents.OnGamePaused += OnGamePaused;
        }
        public void OnDisable()
        {
            GameEvents.OnGameResumed -= OnGameResume;
            GameEvents.OnGamePaused -= OnGamePaused;
        }

        private void OnGamePaused()
        {
            if(!_isPauseble) return;
            _active = false;
            audioSource.Pause();
        }


        private void OnGameResume()
        {
            if(!_isPauseble) return;
            _active = true;
            audioSource.UnPause();
        }

        public void Update()
        {
            if (_active && !audioSource.isPlaying)
            {
                if(_callback != null) _callback();
                _soundPool.Return(this);
            }
        }

        public void Play(Sound sound, Vector3 pos, SoundPool pool, Action callback = null)
        {
            _soundPool = pool;
            gameObject.transform.position = pos;
            audioSource.clip = sound.clip;
            audioSource.volume = sound.volume;
            audioSource.pitch = sound.pitch;
            audioSource.loop = sound.loop;
            audioSource.spatialBlend = sound.spatialBlend;
            _isPauseble = sound.pauseOnGamePause; 
            _callback = callback;
            audioSource.Play();
            _active = true;
        }
        
        public void StopSound()
        {
            if (_active && audioSource.isPlaying) 
            {
                audioSource.Stop();
                _soundPool.Return(this);
            }
        }
        

        public void Reset()
        {
            _active = false;
        }
    }
}
