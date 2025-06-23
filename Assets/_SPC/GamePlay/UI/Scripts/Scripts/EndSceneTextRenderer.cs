using System;
using _SPC.Core.Scripts.InputSystem;
using _SPC.Core.Scripts.Managers;
using _SPC.Core.Scripts.Text;
using UnityEngine.InputSystem;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class EndSceneTextRenderer: SceneTextUI
    {
        private InputSystem_Actions _inputSystem;

        public void OnEnable()
        {
            _inputSystem = InputSystemBuffer.Instance.InputSystem;
            _inputSystem.Player.Attack.performed += FadeAway;
        }
        public void OnDisable()
        {
            _inputSystem.Player.Attack.performed -= FadeAway;
        }

        private void FadeAway(InputAction.CallbackContext obj)
        {
            _inputSystem.Player.Attack.performed -= FadeAway;
            base.FadeAway(() => GameManager.Instance.sceneLoader.LoadSceneWithCallback(1,GameEvents.GameStarted));
        }
    }
}