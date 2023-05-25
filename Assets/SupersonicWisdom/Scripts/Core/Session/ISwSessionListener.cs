namespace SupersonicWisdomSDK
{
    internal interface ISwSessionListener
    {
        #region --- Public Methods ---

        void OnSessionEnded(string sessionId);
        void OnSessionStarted(string sessionId);

        #endregion
    }
}