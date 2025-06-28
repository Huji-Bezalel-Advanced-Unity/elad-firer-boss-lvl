using System;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Base class for all attack systems, providing common functionality for entity attacks.
    /// </summary>
    public abstract class SPCAttack
    {
        /// <summary>
        /// Executes the attack. Must be implemented by derived classes.
        /// </summary>
        /// <param name="onFinish">Callback to invoke when the attack finishes.</param>
        /// <returns>True if the attack was successfully executed, false otherwise.</returns>
        public abstract bool Attack(Action onFinish = null);
    }
}