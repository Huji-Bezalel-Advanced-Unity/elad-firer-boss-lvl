using TMPro;
using UnityEngine;

namespace _SPC.GamePlay.UI.Scripts.Scripts
{
    public class HighScoreEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _rankText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void Initialize(int rank, string nickname, int score)
        {
            _rankText.text = rank.ToString();
            _nameText.text = nickname;
            _scoreText.text = score.ToString();
        }
    }
} 