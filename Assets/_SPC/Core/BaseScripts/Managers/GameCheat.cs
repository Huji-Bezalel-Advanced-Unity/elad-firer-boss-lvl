using _SPC.Core.BaseScripts.InputSystem.Scripts;

namespace _SPC.Core.BaseScripts.Managers
{
    /// <summary>
    /// Handles cheat/debug input for triggering game events (e.g., instant win).
    /// </summary>
    public class GameCheat
    {
        private readonly InputSystem_Actions _inputSystem;

        /// <summary>
        /// Subscribes to the GameWin input action to trigger a game win event.
        /// </summary>
        public GameCheat()
        {
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.GameWin.performed += context => GameEvents.GameFinished();
        }
    }
}