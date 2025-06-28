using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Handles the rendering of floating point values above game objects in world space.
    /// </summary>
    public class PointsAboveObjectsRenderer : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("TextMeshPro prefab for displaying points in world space.")]
        [SerializeField] private TextMeshProUGUI pointsTextPrefab;
        
        [Tooltip("Transform of the world space canvas for positioning text elements.")]
        [SerializeField] private Transform worldSpaceCanvasTransform;
        
        [Header("Animation Settings")]
        [Tooltip("Time in seconds before the points text disappears.")]
        [SerializeField] [Range(0.1f, 5f)] private float timeUntilDispawn;

        /// <summary>
        /// Subscribes to points rendering events when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            SubscribeToPointsEvents();
        }

        /// <summary>
        /// Unsubscribes from points rendering events when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            UnsubscribeFromPointsEvents();
        }

        /// <summary>
        /// Renders points text at the specified world position.
        /// </summary>
        /// <param name="worldPosition">The world position where points should be displayed.</param>
        /// <param name="value">The point value to display (positive or negative).</param>
        private void RenderPoints(Vector3 worldPosition, long value)
        {
            if (!ValidateReferences()) return;
            
            var textObj = CreatePointsText();
            PositionTextInWorld(textObj, worldPosition);
            SetPointsText(textObj, value);
            DestroyTextAfterDelay(textObj);
        }

        /// <summary>
        /// Validates that required references are assigned.
        /// </summary>
        /// <returns>True if all references are valid, false otherwise.</returns>
        private bool ValidateReferences()
        {
            return pointsTextPrefab != null && worldSpaceCanvasTransform != null;
        }

        /// <summary>
        /// Creates a new points text GameObject from the prefab.
        /// </summary>
        /// <returns>The created TextMeshPro component.</returns>
        private TextMeshProUGUI CreatePointsText()
        {
            return Instantiate(pointsTextPrefab, worldSpaceCanvasTransform);
        }

        /// <summary>
        /// Positions the text object at the specified world position in canvas space.
        /// </summary>
        /// <param name="textObj">The text object to position.</param>
        /// <param name="worldPosition">The world position to convert to canvas space.</param>
        private void PositionTextInWorld(TextMeshProUGUI textObj, Vector3 worldPosition)
        {
            Vector3 localPos = worldSpaceCanvasTransform.InverseTransformPoint(worldPosition);
            textObj.rectTransform.localPosition = localPos;
        }

        /// <summary>
        /// Sets the text content with appropriate formatting for positive or negative values.
        /// </summary>
        /// <param name="textObj">The text object to update.</param>
        /// <param name="value">The value to display.</param>
        private void SetPointsText(TextMeshProUGUI textObj, long value)
        {
            if (value > 0)
            {
                textObj.text = "+" + value;
            }
            else
            {
                textObj.text = value.ToString();
            }
        }

        /// <summary>
        /// Destroys the text object after the specified delay.
        /// </summary>
        /// <param name="textObj">The text object to destroy.</param>
        private void DestroyTextAfterDelay(TextMeshProUGUI textObj)
        {
            Destroy(textObj.gameObject, timeUntilDispawn);
        }

        /// <summary>
        /// Subscribes to points rendering events from the gameplay combinator.
        /// </summary>
        private void SubscribeToPointsEvents()
        {
            GameplayCombinator.RenderPoints += RenderPoints;
        }

        /// <summary>
        /// Unsubscribes from points rendering events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromPointsEvents()
        {
            GameplayCombinator.RenderPoints -= RenderPoints;
        }
    }
}