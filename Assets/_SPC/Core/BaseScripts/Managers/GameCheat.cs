using _SPC.Core.BaseScripts.InputSystem.Scripts;

namespace _SPC.Core.BaseScripts.Managers
{
    public class GameCheat
    {
        private readonly InputSystem_Actions _inputSystem;

        public GameCheat()
        {
            _inputSystem =InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.GameWin.performed += context => GameEvents.GameFinished();
        }
    }
}