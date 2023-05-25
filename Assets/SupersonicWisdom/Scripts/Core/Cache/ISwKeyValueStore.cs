namespace SupersonicWisdomSDK
{
    public interface ISwKeyValueStore
    {
        #region --- Public Methods ---

        void DeleteAll ();

        void DeleteKey(string key);
        bool GetBoolean(string key, bool defaultValue = false);
        float GetFloat(string key, float defaultValue = 0f);
        int GetInt(string key, int defaultValue = 0);
        string GetString(string key, string defaultValue = "");

        bool HasKey(string key);

        void Save ();
        ISwKeyValueStore SetBoolean(string key, bool value);
        ISwKeyValueStore SetFloat(string key, float value);
        ISwKeyValueStore SetInt(string key, int value);

        ISwKeyValueStore SetString(string key, string value);

        #endregion
    }
}