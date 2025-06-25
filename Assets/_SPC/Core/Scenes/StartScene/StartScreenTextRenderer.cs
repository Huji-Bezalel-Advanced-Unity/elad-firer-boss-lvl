using System;
using _SPC.Core.Scenes.SceneBaseScripts;
using _SPC.GamePlay.UI.Scripts.Scripts;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _SPC.Core.Scripts.Text
{
    public class StartScreenTextRenderer: SceneTextUI
    {
        [Header("logo fields")]
        [SerializeField] private GameObject logo;
        [SerializeField] private float logoMoveTime = 3f;
        [SerializeField] private Vector3 endPositionForLogo;
        
        public override void FadeAway(Action action)
        {
            tween.Kill();
            var sequence = SequenceFadeText(action);
            sequence.Append(logo.transform.DOMove(endPositionForLogo, logoMoveTime));
            sequence.Play();
        }
        
    }
}