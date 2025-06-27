using System;
using System.Collections.Generic;
using _SPC.Core.BaseScripts.Generics.MonoSingletone;
using UnityEngine.SceneManagement;

using UnityEngine;

namespace _SPC.Core.Audio
{
    public class AudioManager : SpcMonoSingleton<AudioManager>
    {
        [SerializeField] SoundPool soundPool;
        public Sound[] sounds;

        private Dictionary<AudioName,List<SoundObject>> soundDictionary = new Dictionary<AudioName, List<SoundObject>>();
        private int _lastSceneIndex = -1;

        private void Awake()
        {
            _lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }

        private void Update()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex != _lastSceneIndex)
            {
                _lastSceneIndex = currentSceneIndex;
                StopSceneBoundSounds();
            }
        }

        private void StopSceneBoundSounds()
        {
            foreach (var soundListKey in soundDictionary)
            {
                // Find the Sound definition for this AudioName
                Sound soundDef = Array.Find(sounds, s => s.name == soundListKey.Key);
                if (soundDef != null && soundDef.stopWhenMovingToNextScene)
                {
                    foreach (var soundObj in soundListKey.Value)
                    {
                        soundObj.StopSound();
                    }
                    soundListKey.Value.Clear();
                }
            }
        }

        public SoundObject Play(AudioName name, Vector3 pos, Action callback = null)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return null;
            }
            
            SoundObject soundObject = soundPool.Get();
            soundDictionary.TryAdd(name, new List<SoundObject>());
            soundDictionary[name].Add(soundObject);
            soundObject.Play(s,pos,soundPool,callback);
            return soundObject;
        }

        public void Stop(AudioName name)
        {
            soundDictionary.TryGetValue(name, out List<SoundObject> soundObjects);
            foreach (var sound in soundObjects)
            {
                sound.StopSound();
            }
            soundObjects.Clear();
        }

        public void StopAll()
        {
            foreach (var soundListKey in soundDictionary)
            {
                foreach (var sound in soundListKey.Value)
                {
                    sound.StopSound();
                }
            }
            soundDictionary.Clear();
        }
    
    }

    public enum AudioName
    {
        GameStartMusic,
        GamePlayMusic,
        GameLossMusicScene,
        GameWinMusicScene,
        PlayerShotMusic,
        PlayerSuccessfulShotMusic,
        EnemySuccessfulShotMusic,
        EnemyShotMusic,
        EnemySpecialShotMusic,
        GameOverMusic,
        GameWinMusic,
    }
}