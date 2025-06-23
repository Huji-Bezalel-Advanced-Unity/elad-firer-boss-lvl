using System.Collections.Generic;
using _SPC.GamePlay.Player.Scripts.Controllers;
using _SPC.GamePlay.Enemies.Boss.Scripts.Controllers;

namespace _SPC.Core.Scripts.Abstracts
{
    public abstract class SPCStatsUpgrader
    {
        // These will be initialized in the derived classes using their specific UpgradeType enums
        public static Dictionary<PlayerStatsUpgrader.UpgradeType, int> PlayerUpgradeCounts = new Dictionary<PlayerStatsUpgrader.UpgradeType, int>();
        public static Dictionary<BossStatsUpgrader.UpgradeType, int> BossUpgradeCounts = new Dictionary<BossStatsUpgrader.UpgradeType, int>();
        public abstract void ResetStats();
    }
} 