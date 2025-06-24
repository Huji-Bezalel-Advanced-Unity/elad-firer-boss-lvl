using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class SceneTextUI: MonoBehaviour
    {
        [Header("Text Fields")]
        [SerializeField] private float _fadeTime = 2f;
        [SerializeField] protected TextMeshProUGUI _text;
        protected TweenerCore<Color, Color, ColorOptions> tween;
        private void Start()
        {
            tween = _text.DOFade(0f, 2f).SetLoops(-1, LoopType.Yoyo);
        }

        public virtual void FadeAway(Action action)
        {
            tween.Kill();
            var sequence = SequenceFadeText(action);
            sequence.Play();
        }
        
        protected Sequence SequenceFadeText(Action action)
        {
            float currentAlpha = _text.color.a;
            float duration = _fadeTime * currentAlpha;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_text.DOFade(0f, duration));
            sequence.OnComplete(() => {
                action?.Invoke();
                Destroy(gameObject);
            });
            return sequence;
        }
    }
}