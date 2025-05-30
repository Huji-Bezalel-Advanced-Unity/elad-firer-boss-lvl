using _SPC.Core.Scripts.Abstracts;
using _SPC.GamePlay.Managers;
using _SPC.GamePlay.Utils;

namespace _SPC.GamePlay.Player.Scripts.Controllers
{
    public class PlayerHealth: SPCHealth
    {
        public PlayerHealth(HealthDependencies dependencies)
            : base(dependencies)
        {
            GameEvents.OnEnemyHit += ReduceLife;
            OnDeathAction += GameEvents.GameFinished;
        }
    }
}