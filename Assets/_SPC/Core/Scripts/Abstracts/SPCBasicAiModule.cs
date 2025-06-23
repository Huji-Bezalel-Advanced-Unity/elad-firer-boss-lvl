using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;

namespace _SPC.Core.Scripts.Abstracts
{
    public abstract class SpcBasicAiModule
    {
        public abstract void Fit();
        public abstract object Predict();

        public abstract void Clear();
    }
} 