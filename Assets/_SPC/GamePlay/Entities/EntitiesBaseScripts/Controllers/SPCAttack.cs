using System;

namespace _SPC.GamePlay.Entities
{
    public abstract class SPCAttack
    {
        public abstract bool Attack(Action onFinish = null);
    }
}