using System.Collections.Generic;
using _SPC.GamePlay.Entities.Enemies.Boss;
using _SPC.GamePlay.Entities.Player;

namespace _SPC.GamePlay.Entities
{
    /// <summary>
    /// Base class for stats upgrade systems, providing common functionality for tracking upgrade counts.
    /// </summary>
    public abstract class SPCStatsUpgrader
    {
        /// <summary>
        /// Resets all stats to their initial values. Must be implemented by derived classes.
        /// </summary>
        public abstract void ResetStats();
    }
} 