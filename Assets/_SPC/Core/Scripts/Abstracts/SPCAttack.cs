using System;

namespace _SPC.Core.Scripts.Abstracts
{
    public abstract class SPCAttack
    {
        public abstract bool Attack(Action onFinish = null);
    }
}