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
        /// Dictionary tracking the count of each player upgrade type.
        /// </summary>
        public static Dictionary<PlayerStatsUpgrader.UpgradeType, int> PlayerUpgradeCounts = new Dictionary<PlayerStatsUpgrader.UpgradeType, int>();
        
        /// <summary>
        /// Dictionary tracking the count of each boss upgrade type.
        /// </summary>
        public static Dictionary<BossStatsUpgrader.UpgradeType, int> BossUpgradeCounts = new Dictionary<BossStatsUpgrader.UpgradeType, int>();
        
        /// <summary>
        /// Resets all stats to their initial values. Must be implemented by derived classes.
        /// </summary>
        public abstract void ResetStats();
    }
} 