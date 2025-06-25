using _SPC.Core.BaseScripts.Managers;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    public class HighScoreRenderer : MonoBehaviour
    {
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private GameObject _highScoreEntryPrefab;
        
        private void OnEnable()
        {
            GameEvents.OnEndSceneStarted += RenderHighScores;
        }

        private void OnDisable()
        {
            GameEvents.OnEndSceneStarted  -= RenderHighScores;
        }

        private void RenderHighScores()
        {
            var highScoreManager = GameManager.Instance.HighScoreManager;
            var table = highScoreManager.GetHighScoreTable();
            
            foreach (var entry in table)
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
        }
    }
} 