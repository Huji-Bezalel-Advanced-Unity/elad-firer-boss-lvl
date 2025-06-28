using _SPC.Core.BaseScripts.Managers;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Handles the rendering and display of high score entries in the UI grid.
    /// </summary>
    public class HighScoreRenderer : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Transform container for organizing high score entries in a grid layout.")]
        [SerializeField] private Transform _gridContainer;
        
        [Tooltip("Prefab for individual high score entry UI elements.")]
        [SerializeField] private GameObject _highScoreEntryPrefab;
        
        /// <summary>
        /// Subscribes to end scene events when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            SubscribeToEndSceneEvents();
        }

        /// <summary>
        /// Unsubscribes from end scene events when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            UnsubscribeFromEndSceneEvents();
        }

        /// <summary>
        /// Renders the high score table by creating UI entries for each score.
        /// </summary>
        private void RenderHighScores()
        {
            var highScoreManager = GameManager.Instance.HighScoreManager;
            var table = highScoreManager.GetHighScoreTable();
            
            CreateHighScoreEntries(table);
        }

        /// <summary>
        /// Creates UI entries for each high score in the table.
        /// </summary>
        /// <param name="table">The high score table containing rank, nickname, and score data.</param>
        private void CreateHighScoreEntries(System.Collections.Generic.List<(int Rank, long Score, string Nickname)> table)
        {
            foreach (var entry in table)
            {
                CreateHighScoreEntry(entry);
            }
        }

        /// <summary>
        /// Creates a single high score entry UI element.
        /// </summary>
        /// <param name="entry">The high score entry data containing rank, nickname, and score.</param>
        private void CreateHighScoreEntry((int Rank, long Score, string Nickname) entry)
        {
            var entryGO = Instantiate(_highScoreEntryPrefab, _gridContainer);
            var entryUI = entryGO.GetComponent<HighScoreEntryUI>();
            
            if (entryUI != null)
            {
                entryUI.Initialize(entry.Rank, entry.Nickname, entry.Score);
            }
            else
            {
                Debug.LogError("HighScoreEntry prefab is missing the HighScoreEntryUI script.", entryGO);
            }
        }

        /// <summary>
        /// Subscribes to end scene events for high score rendering.
        /// </summary>
        private void SubscribeToEndSceneEvents()
        {
            GameEvents.OnEndSceneStarted += RenderHighScores;
        }

        /// <summary>
        /// Unsubscribes from end scene events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEndSceneEvents()
        {
            GameEvents.OnEndSceneStarted -= RenderHighScores;
        }
    }
} 