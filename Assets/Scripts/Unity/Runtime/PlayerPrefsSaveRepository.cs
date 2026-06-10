using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public interface ISaveRepository
    {
        SaveData Load();
        void Save(SaveData saveData);
        void Reset();
    }

    public sealed class PlayerPrefsSaveRepository : ISaveRepository
    {
        private const string SaveKey = "PotionPopQuest.SaveData.v1";
        private readonly IGameLogger _logger;

        public PlayerPrefsSaveRepository(IGameLogger logger)
        {
            _logger = logger;
        }

        public SaveData Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                _logger.Log(LogCategory.Save, "No save found; creating default save.");
                return new SaveData();
            }

            var json = PlayerPrefs.GetString(SaveKey);
            var save = JsonUtility.FromJson<SaveData>(json);
            _logger.Log(LogCategory.Save, "Loaded local save data.");
            return save ?? new SaveData();
        }

        public void Save(SaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            _logger.Log(LogCategory.Save, "Saved local progress.");
        }

        public void Reset()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            _logger.Warn(LogCategory.Save, "Reset local progress.");
        }
    }
}

