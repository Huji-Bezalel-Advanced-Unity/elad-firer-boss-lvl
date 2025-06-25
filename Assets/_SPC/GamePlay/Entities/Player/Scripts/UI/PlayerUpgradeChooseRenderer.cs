using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Entities.Player
{
    public class PlayerUpgradeChooseRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _leftChoiceText;
        [SerializeField] private TextMeshProUGUI _rightChoiceText;
        [SerializeField] private float _fadeDuration = 0.5f;

        private void Awake()
        {
            _leftChoiceText.alpha = 0;
            _rightChoiceText.alpha = 0;
            _leftChoiceText.gameObject.SetActive(false);
            _rightChoiceText.gameObject.SetActive(false);
        }

        public void RenderChoices(string leftChoiceText, string rightChoiceText, Action onComplete = null)
        {
            _leftChoiceText.text = $"Press L1 / 1 To Activate:\n{leftChoiceText}";
            _rightChoiceText.text = $"Press R1 / 2 To Activate:\n{rightChoiceText}";
            
            _leftChoiceText.gameObject.SetActive(true);
            _rightChoiceText.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(_leftChoiceText.DOFade(1, _fadeDuration));
            sequence.Join(_rightChoiceText.DOFade(1, _fadeDuration));
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        public void UnrenderChoices(Action onComplete = null)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_leftChoiceText.DOFade(0, _fadeDuration));
            sequence.Join(_rightChoiceText.DOFade(0, _fadeDuration));
            sequence.OnComplete(() =>
            {
                _leftChoiceText.gameObject.SetActive(false);
                _rightChoiceText.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
    }
} 