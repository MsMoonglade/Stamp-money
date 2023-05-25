using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwPlayerPrefsStore : ISwKeyValueStore
    {
        #region --- Public Methods ---

        public void DeleteAll ()
        {
            SwInfra.Logger.Log("SwPlayerPrefsStore | DeleteAll");
            PlayerPrefs.DeleteAll();
        }

        public void DeleteKey(string key)
        {
            SwInfra.Logger.Log($"SwPlayerPrefsStore | DeleteKey | {key}");
            PlayerPrefs.DeleteKey(key);
        }

        public bool GetBoolean(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void Save ()
        {
            PlayerPrefs.Save();
        }

        public ISwKeyValueStore SetBoolean(string key, bool value)
        {
            SwInfra.Logger.Log($"SwPlayerPrefsStore | SetBoolean | {key} | {value}");
            PlayerPrefs.SetInt(key, value ? 1 : 0);

            return this;
        }

        public ISwKeyValueStore SetFloat(string key, float value)
        {
            SwInfra.Logger.Log($"SwPlayerPrefsStore | SetFloat | {key} | {value}");
            PlayerPrefs.SetFloat(key, value);

            return this;
        }

        public ISwKeyValueStore SetInt(string key, int value)
        {
            SwInfra.Logger.Log($"SwPlayerPrefsStore | SetInt | {key} | {value}");
            PlayerPrefs.SetInt(key, value);

            return this;
        }

        public ISwKeyValueStore SetString(string key, string value)
        {
            SwInfra.Logger.Log($"SwPlayerPrefsStore | SetString | {key} | {value}");
            PlayerPrefs.SetString(key, value);

            return this;
        }

        #endregion
    }
}