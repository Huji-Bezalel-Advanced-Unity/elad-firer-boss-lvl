using System;
using _SPC.Core.Scripts.InputSystem;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.Core.Scripts.Text
{
    public class StartScreenTextRenderer: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private TweenerCore<Color, Color, ColorOptions> tween;
        [SerializeField] private float _fadeTime = 2f;
        [SerializeField] private GameObject logo;
        [SerializeField] private float logoMoveTime = 3f;
        [SerializeField] private Vector3 endPositionForLogo;

        private void Start()
        {
            tween = _text.DOFade(0f, 2f).SetLoops(-1, LoopType.Yoyo);
        }


        public void FadeAway(Action action)
        {
            tween.Kill();
            float currentAlpha = _text.color.a;
            float duration = _fadeTime * currentAlpha;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_text.DOFade(0f, duration));
            sequence.Append(logo.transform.DOMove(endPositionForLogo, logoMoveTime));
            sequence.OnComplete(() =>
            {
                action?.Invoke();
                Destroy(gameObject);
            });
            sequence.Play();
        }
    }
}