using System.Collections;

namespace SupersonicWisdomSDK
{
    internal interface ISwBlockingApiMiddleware
    {
        #region --- Public Methods ---

        IEnumerator ProcessGameStarted ();
        IEnumerator ProcessLevelCompleted(long level, string levelName);
        IEnumerator ProcessLevelFailed(long level, string levelName);
        IEnumerator ProcessLevelRevived(long level, string levelName);
        IEnumerator ProcessLevelSkipped(long level, string levelName);
        IEnumerator ProcessLevelStarted(long level, string levelName);
        IEnumerator ProcessRewardedVideoOpportunityMissed ();

        #endregion
    }
}