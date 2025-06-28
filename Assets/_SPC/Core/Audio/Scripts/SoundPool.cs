using _SPC.Core.BaseScripts.Generics.MonoPool;

namespace _SPC.Core.Audio
{
    /// <summary>
    /// Pool for SoundObject instances, enabling efficient reuse of audio sources.
    /// </summary>
    public class SoundPool : SPCMonoPool<SoundObject>
    {
        // No additional logic needed; inherits pooling behavior from SPCMonoPool.
    }
}