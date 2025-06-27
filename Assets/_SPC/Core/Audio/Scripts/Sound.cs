using UnityEngine;

namespace _SPC.Core.Audio
{
    [System.Serializable]
    public class Sound
    {
        public AudioName name;
        
        public AudioClip clip;
        [Range(0.0f, 1.0f)]
        public float volume;
        [Range(.1f, 3.0f)]
        public float pitch;
        public float spatialBlend;
    
        public bool loop;
        public bool stopWhenMovingToNextScene;
        public bool pauseOnGamePause;
    }
}