using System.Text;
using _SPC.GamePlay.Managers;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class HighScoreRenderer : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnEndSceneStarted += PrintHighScores;
        }

        private void OnDisable()
        {
            GameEvents.OnEndSceneStarted  -= PrintHighScores;
        }

        private void PrintHighScores()
        {
            var highScoreManager = GameManager.Instance.HighScoreManager;
            var table = highScoreManager.GetHighScoreTable();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("High Scores:");
            foreach (var entry in table)
            {
                sb.AppendLine($"{entry.Rank}. {entry.Nickname} - {entry.Score}");
            }
            Debug.Log(sb.ToString());
        }
    }
} 