using System;
using System.Collections.Generic;
using UnityEngine;

namespace _SPC.GamePlay.Score
{
    /// <summary>
    /// Manages high score persistence, retrieval, and ranking for the game.
    /// </summary>
    public class HighScoreManager
    {
        private const int MaxHighScores = 10;
        private const string PlayerPrefsKey = "HighScoreList";
        
        private HighScoreList _highScoreList;

        /// <summary>
        /// Initializes the high score manager and loads existing high scores.
        /// </summary>
        public HighScoreManager()
        {
            LoadHighScores();
        }

        /// <summary>
        /// Attempts to add a new high score entry, updating existing entries if the score is higher.
        /// </summary>
        /// <param name="score">The score to potentially add.</param>
        /// <param name="nickname">The player's nickname (defaults to "Player" if empty).</param>
        public void TryAddHighScore(long score, string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                nickname = "Player";

            var existingEntry = FindExistingEntry(nickname);

            if (existingEntry != null)
            {
                UpdateExistingEntry(existingEntry, score);
            }
            else
            {
                AddNewEntry(score, nickname);
            }

            SortAndTrimHighScores();
            SaveHighScores();
        }

        /// <summary>
        /// Gets the high score table for UI display.
        /// </summary>
        /// <returns>A list of tuples containing rank, score, and nickname for each entry.</returns>
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

        /// <summary>
        /// Saves the current high score list to PlayerPrefs.
        /// </summary>
        private void SaveHighScores()
        {
            string json = JsonUtility.ToJson(_highScoreList);
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads high scores from PlayerPrefs or creates a new list if none exist.
        /// </summary>
        private void LoadHighScores()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                LoadExistingHighScores();
            }
            else
            {
                CreateNewHighScoreList();
            }
        }

        /// <summary>
        /// Loads existing high scores from PlayerPrefs.
        /// </summary>
        private void LoadExistingHighScores()
        {
            var json = PlayerPrefs.GetString(PlayerPrefsKey);
            _highScoreList = JsonUtility.FromJson<HighScoreList>(json) ?? new HighScoreList();
        }

        /// <summary>
        /// Creates a new high score list when no existing data is found.
        /// </summary>
        private void CreateNewHighScoreList()
        {
            _highScoreList = new HighScoreList();
        }

        /// <summary>
        /// Finds an existing high score entry for the given nickname.
        /// </summary>
        /// <param name="nickname">The nickname to search for.</param>
        /// <returns>The existing entry if found, null otherwise.</returns>
        private HighScoreEntry FindExistingEntry(string nickname)
        {
            return _highScoreList.Scores.Find(entry => entry.Nickname == nickname);
        }

        /// <summary>
        /// Updates an existing high score entry if the new score is higher.
        /// </summary>
        /// <param name="existingEntry">The existing entry to potentially update.</param>
        /// <param name="newScore">The new score to compare against.</param>
        private void UpdateExistingEntry(HighScoreEntry existingEntry, long newScore)
        {
            if (newScore > existingEntry.Score)
            {
                existingEntry.Score = newScore;
            }
        }

        /// <summary>
        /// Adds a new high score entry to the list.
        /// </summary>
        /// <param name="score">The score for the new entry.</param>
        /// <param name="nickname">The nickname for the new entry.</param>
        private void AddNewEntry(long score, string nickname)
        {
            HighScoreEntry entry = new HighScoreEntry(score, nickname);
            _highScoreList.Scores.Add(entry);
        }

        /// <summary>
        /// Sorts the high scores in descending order and removes excess entries beyond the maximum limit.
        /// </summary>
        private void SortAndTrimHighScores()
        {
            _highScoreList.Scores.Sort((a, b) => b.Score.CompareTo(a.Score));
            
            if (_highScoreList.Scores.Count > MaxHighScores)
            {
                _highScoreList.Scores.RemoveRange(MaxHighScores, _highScoreList.Scores.Count - MaxHighScores);
            }
        }
    }

    /// <summary>
    /// Serializable container for a list of high score entries.
    /// </summary>
    [Serializable]
    public class HighScoreList
    {
        /// <summary>
        /// List of high score entries.
        /// </summary>
        public List<HighScoreEntry> Scores = new();
    }
    
    /// <summary>
    /// Represents a single high score entry with score and nickname.
    /// </summary>
    [Serializable]
    public class HighScoreEntry
    {
        /// <summary>
        /// The score value for this entry.
        /// </summary>
        public long Score;
        
        /// <summary>
        /// The player's nickname for this entry.
        /// </summary>
        public string Nickname;

        /// <summary>
        /// Initializes a new high score entry.
        /// </summary>
        /// <param name="score">The score value.</param>
        /// <param name="nickname">The player's nickname.</param>
        public HighScoreEntry(long score, string nickname)
        {
            Score = score;
            Nickname = nickname;
        }
    }
}