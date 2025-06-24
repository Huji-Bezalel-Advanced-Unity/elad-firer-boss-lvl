using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    public class HighScoreManager
{
    private const int MaxHighScores = 10;
    private const string PlayerPrefsKey = "HighScoreList";
    
    private HighScoreList _highScoreList;

    public HighScoreManager()
    {
        LoadHighScores();
    }

    // Call this when player finishes and wants to submit score
    public void TryAddHighScore(long score, string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            nickname = "Player";

        // Check if player already has an entry
        var existingEntry = _highScoreList.Scores.Find(entry => entry.Nickname == nickname);

        if (existingEntry != null)
        {
            // Only update if the new score is higher
            if (score > existingEntry.Score)
            {
                existingEntry.Score = score;
            }
            else
            {
                return; // New score is not higher, do nothing
            }
        }
        else
        {
            // No existing entry, add new
            HighScoreEntry entry = new HighScoreEntry(score, nickname);
            _highScoreList.Scores.Add(entry);
        }

        // Sort and keep only top 10
        _highScoreList.Scores.Sort((a, b) => b.Score.CompareTo(a.Score));
        if (_highScoreList.Scores.Count > MaxHighScores)
            _highScoreList.Scores.RemoveRange(MaxHighScores, _highScoreList.Scores.Count - MaxHighScores);

        SaveHighScores();
    }


    // Saving/loading
    private void SaveHighScores()
    {
        string json = JsonUtility.ToJson(_highScoreList);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            var json = PlayerPrefs.GetString(PlayerPrefsKey);
            _highScoreList = JsonUtility.FromJson<HighScoreList>(json) ?? new HighScoreList();
        }
        else
        {
            _highScoreList = new HighScoreList();
        }
    }

    // For displaying the high scores (returns list for UI)
    public List<(int Rank, long Score, string Nickname)> GetHighScoreTable()
    {
        var table = new List<(int, long, string)>();
        for (int i = 0; i < _highScoreList.Scores.Count; i++)
        {
            var entry = _highScoreList.Scores[i];
            table.Add((i + 1, entry.Score, entry.Nickname));
        }
        return table;
    }
}

    [Serializable]
    public class HighScoreList
    {
        public List<HighScoreEntry> Scores = new();
    }
    
    [Serializable]
    public class HighScoreEntry
    {
        public long Score;
        public string Nickname;

        public HighScoreEntry(long score, string nickname)
        {
            Score = score;
            Nickname = nickname;
        }
    }
}