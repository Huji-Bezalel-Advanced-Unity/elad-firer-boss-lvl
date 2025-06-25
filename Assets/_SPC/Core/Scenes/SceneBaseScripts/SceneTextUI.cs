using System;
using _SPC.Core.BaseScripts;
using _SPC.Core.BaseScripts.BaseMono;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace _SPC.Core.Scenes.SceneBaseScripts
{
    public class SceneTextUI: SPCBaseMono
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