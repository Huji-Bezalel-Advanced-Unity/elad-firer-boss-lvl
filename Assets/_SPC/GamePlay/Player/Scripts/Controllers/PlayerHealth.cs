using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public class PlayerHealth: SPCHealth
    {
        public PlayerHealth(GameLogger playerLogger)
        {
            GameEvents.OnEnemyHit += ReduceLife;
            OnDeathAction += GameEvents.GameFinished;
            logger = playerLogger;
        }
    }
}