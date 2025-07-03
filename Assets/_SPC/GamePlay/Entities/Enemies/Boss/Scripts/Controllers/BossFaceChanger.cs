using UnityEngine;

namespace _SPC.GamePlay.Entities.Enemies.Boss
{
    /// <summary>
    /// Holds dependencies for the BossFaceChanger, including the boss sprite renderer and face sprites.
    /// </summary>
    public struct BossFaceChangerDependencies
    {
        public SpriteRenderer BossSpriteRenderer;
        public Sprite NormalFaceSprite;
        public Sprite AngryFaceSprite;
    }

    /// <summary>
    /// Handles changing the boss's face sprite between normal and angry states.
    /// </summary>
    public class BossFaceChanger
    {
        private readonly BossFaceChangerDependencies _deps;
        private bool _isAngry = false;

        /// <summary>
        /// Initializes the BossFaceChanger with the given dependencies.
        /// </summary>
        public BossFaceChanger(BossFaceChangerDependencies deps)
        {
            _deps = deps;
        }

        /// <summary>
        /// Sets the boss's face to the angry sprite, if not already angry.
        /// </summary>
        public void SetAngryFace()
        {
            if (!_isAngry && _deps.BossSpriteRenderer != null && _deps.AngryFaceSprite != null)
            {
                _deps.BossSpriteRenderer.sprite = _deps.AngryFaceSprite;
                _isAngry = true;
            }
        }

        /// <summary>
        /// Sets the boss's face to the normal sprite, if currently angry.
        /// </summary>
        public void SetNormalFace()
        {
            if (_isAngry && _deps.BossSpriteRenderer != null && _deps.NormalFaceSprite != null)
            {
                _deps.BossSpriteRenderer.sprite = _deps.NormalFaceSprite;
                _isAngry = false;
            }
        }
        
    }
}