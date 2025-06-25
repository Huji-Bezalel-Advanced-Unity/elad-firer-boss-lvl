using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    public struct BossFaceChangerDependencies
    {
        public SpriteRenderer BossSpriteRenderer;
        public Sprite NormalFaceSprite;
        public Sprite AngryFaceSprite;
    }

    public class BossFaceChanger
    {
        private readonly BossFaceChangerDependencies _deps;
        private bool _isAngry = false;

        public BossFaceChanger(BossFaceChangerDependencies deps)
        {
            _deps = deps;
        }

        public void SetAngryFace()
        {
            if (!_isAngry && _deps.BossSpriteRenderer != null && _deps.AngryFaceSprite != null)
            {
                _deps.BossSpriteRenderer.sprite = _deps.AngryFaceSprite;
                _isAngry = true;
            }
        }

        public void SetNormalFace()
        {
            if (_isAngry && _deps.BossSpriteRenderer != null && _deps.NormalFaceSprite != null)
            {
                _deps.BossSpriteRenderer.sprite = _deps.NormalFaceSprite;
                _isAngry = false;
            }
        }

        public void ToggleFace()
        {
            if (_isAngry)
            {
                SetNormalFace();
            }
            else
            {
                SetAngryFace();
            }
        }

        public bool IsAngry => _isAngry;
    }
}