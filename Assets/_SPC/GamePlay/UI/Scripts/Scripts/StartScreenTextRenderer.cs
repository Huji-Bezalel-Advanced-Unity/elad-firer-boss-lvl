using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class StartScreenTextRenderer: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private void Start()
        {
            _text.DOFade(0f, 2f).SetLoops(-1, LoopType.Yoyo);
        }
        
    }
}