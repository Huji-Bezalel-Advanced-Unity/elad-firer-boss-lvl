using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Player
{
    /// <summary>
    /// Handles the rendering and animation of player upgrade choice UI elements.
    /// </summary>
    public class PlayerUpgradeChooseRenderer : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Text component for the left upgrade choice.")]
        [SerializeField] private TextMeshProUGUI _leftChoiceText;
        
        [Tooltip("Text component for the right upgrade choice.")]
        [SerializeField] private TextMeshProUGUI _rightChoiceText;
        
        [Header("Animation Settings")]
        [Tooltip("Duration of fade in/out animations for the upgrade choices.")]
        [SerializeField] private float _fadeDuration = 0.5f;

        /// <summary>
        /// Initializes the UI elements by hiding them and setting their alpha to 0.
        /// </summary>
        private void Awake()
        {
            InitializeUIElements();
        }

        /// <summary>
        /// Renders the upgrade choices with fade-in animation.
        /// </summary>
        /// <param name="leftChoiceText">Text for the left upgrade choice.</param>
        /// <param name="rightChoiceText">Text for the right upgrade choice.</param>
        /// <param name="onComplete">Callback to invoke when the animation completes.</param>
        public void RenderChoices(string leftChoiceText, string rightChoiceText, Action onComplete = null)
        {
            SetChoiceTexts(leftChoiceText, rightChoiceText);
            ShowChoiceElements();
            AnimateFadeIn(onComplete);
        }

        /// <summary>
        /// Hides the upgrade choices with fade-out animation.
        /// </summary>
        /// <param name="onComplete">Callback to invoke when the animation completes.</param>
        public void UnrenderChoices(Action onComplete = null)
        {
            AnimateFadeOut(onComplete);
        }

        /// <summary>
        /// Initializes the UI elements by setting their alpha to 0 and hiding them.
        /// </summary>
        private void InitializeUIElements()
        {
            _leftChoiceText.alpha = 0;
            _rightChoiceText.alpha = 0;
            _leftChoiceText.gameObject.SetActive(false);
            _rightChoiceText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets the text content for both upgrade choices.
        /// </summary>
        /// <param name="leftChoiceText">Text for the left upgrade choice.</param>
        /// <param name="rightChoiceText">Text for the right upgrade choice.</param>
        private void SetChoiceTexts(string leftChoiceText, string rightChoiceText)
        {
            _leftChoiceText.text = $"Press L1 / 1 To Activate:\n{leftChoiceText}";
            _rightChoiceText.text = $"Press R1 / 2 To Activate:\n{rightChoiceText}";
        }

        /// <summary>
        /// Shows the choice UI elements by activating their GameObjects.
        /// </summary>
        private void ShowChoiceElements()
        {
            _leftChoiceText.gameObject.SetActive(true);
            _rightChoiceText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Animates the fade-in effect for both choice texts.
        /// </summary>
        /// <param name="onComplete">Callback to invoke when the animation completes.</param>
        private void AnimateFadeIn(Action onComplete = null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_leftChoiceText.DOFade(1, _fadeDuration));
            sequence.Join(_rightChoiceText.DOFade(1, _fadeDuration));
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        /// <summary>
        /// Animates the fade-out effect for both choice texts and hides them when complete.
        /// </summary>
        /// <param name="onComplete">Callback to invoke when the animation completes.</param>
        private void AnimateFadeOut(Action onComplete = null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_leftChoiceText.DOFade(0, _fadeDuration));
            sequence.Join(_rightChoiceText.DOFade(0, _fadeDuration));
            sequence.OnComplete(() =>
            {
                HideChoiceElements();
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Hides the choice UI elements by deactivating their GameObjects.
        /// </summary>
        private void HideChoiceElements()
        {
            _leftChoiceText.gameObject.SetActive(false);
            _rightChoiceText.gameObject.SetActive(false);
        }
    }
} 