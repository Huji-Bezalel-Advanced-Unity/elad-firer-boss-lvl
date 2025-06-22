using System;
using _SPC.Core.Scripts.InputSystem;
using _SPC.Core.Scripts.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.Core.Scripts.Text
{
    public class StartSceneManager: MonoBehaviour
    {
        [SerializeField] private StartScreenTextRenderer startScreenTextRenderer;
        [SerializeField] private NameInputUI nameInputUI;
        private InputSystem_Actions inputSystem;


        private void OnEnable()
        {
            inputSystem = InputSystemBuffer.Instance.InputSystem;
            inputSystem.Player.Attack.performed += HandleTextInput;
            nameInputUI.OnSubmitName += ActivateGame;
        }

        private void ActivateGame(string name)
        {
            GameManager.Instance.sceneLoader.LoadSceneWithCallback(1,GameEvents.GameStarted);
        }

        private void HandleTextInput(InputAction.CallbackContext obj)
        {
            inputSystem.Player.Attack.performed -= HandleTextInput;
            startScreenTextRenderer.FadeAway(() => nameInputUI.gameObject.SetActive(true));
        }
    }
}