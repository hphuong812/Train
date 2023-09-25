using System;
using System.Collections.Generic;
using ADN.Meta.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace IsoMatrix.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameConfig/GameData")]
    public class GameConfigLoader : ScriptableSingleton<GameConfigLoader>
    {
        public enum LoadState
        {
            Preload,
            Loading,
            Loaded
        }

        [SerializeField] private string levelKey = "Level";

        [SerializeField] private TextAsset levelDataDefault;

        private Dictionary<string, object> defaultLevelConfig;

        [NonSerialized] public LoadState state = LoadState.Preload;
        [NonSerialized] public UnityAction Loaded;

        public void Load()
        {
            state = LoadState.Loading;
            LoadDefaultConfig();
            LoadConfig();
        }

        private void LoadDefaultConfig()
        {

            defaultLevelConfig = new Dictionary<string, object>()
            {
                { levelKey, levelDataDefault.text }
            };
        }

        private void LoadConfig()
        {
            LoadChapterData();
            state = LoadState.Loaded;
            Loaded?.Invoke();
        }

        private void LoadChapterData()
        {
            var data = LoadLevelConfigRawData<List<LevelItemData>>(levelKey);

            if (data != null)
            {
                GameConfig.Instance.LevelItemList = data;
            }
        }

        private TData LoadLevelConfigRawData<TData>(string key)
        {
            var rawData = LoadLevelConfigData(key);
            GetConfigData<TData>(rawData, out var data);
            return data;
        }

        private string LoadLevelConfigData(string key)
        {
            var configKey = key;
            object value;
            defaultLevelConfig.TryGetValue(configKey, out value);
            string data = value as string;
            return data;
        }

        private void GetConfigData<TData>(string json, out TData dest)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError($"Empty config data");
                dest = default;
            }

            var configData = GetConfigData<TData>(json);
            if (configData == null || configData.data == null)
            {
                dest = default;
            }

            dest = configData.data;
        }

        private GameConfigData<TData> GetConfigData<TData>(string jsonData)
        {
            return JsonConvert.DeserializeObject<GameConfigData<TData>>(jsonData);
        }
    }
}
