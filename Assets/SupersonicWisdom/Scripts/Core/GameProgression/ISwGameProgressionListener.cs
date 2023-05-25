namespace SupersonicWisdomSDK
{
    internal interface ISwGameProgressionListener
    {
        #region --- Public Methods ---

        void OnTimeBasedGameStarted ();
        void OnLevelCompleted(long level, string levelName, long attempts, long revives);
        void OnLevelFailed(long level, string levelName, long attempts, long revives);
        void OnLevelRevived(long level, string levelName, long attempts, long revives);
        void OnLevelSkipped(long level, string levelName, long attempts, long revives);
        void OnLevelStarted(long level, string levelName, long attempts, long revives);

        #endregion
    }
}