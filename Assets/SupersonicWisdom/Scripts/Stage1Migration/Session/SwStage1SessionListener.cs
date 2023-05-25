#if SW_STAGE_STAGE1_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage1SessionListener : ISwSessionListener
    {
        #region --- Members ---

        private readonly SwUserData _userData;

        #endregion


        #region --- Construction ---

        public SwStage1SessionListener(SwUserData userData)
        {
            _userData = userData;
        }

        #endregion


        #region --- Public Methods ---

        public virtual void OnSessionEnded(string sessionId)
        {
            SwInfra.Logger.Log($"Unity:SwSessions:OnSessionEnded:{sessionId}");
        }

        public virtual void OnSessionStarted(string sessionId)
        {
            UpdateUserStateOnStartSession(sessionId);
            SwInfra.Logger.Log($"Unity:SwSessions:OnSessionStarted:{sessionId}");
        }

        #endregion


        #region --- Private Methods ---

        private void UpdateUserStateOnStartSession(string sessionId)
        {
            _userData.ModifyUserStateSync(mutableUserState =>
            {
                _userData.UpdateAge(mutableUserState);
                mutableUserState.SessionId = sessionId;
                mutableUserState.todaySessionsCount++;
                mutableUserState.totalSessionsCount++;
            });

            _userData.AfterUserStateChangeInternal(SwUserStateChangeReason.SessionStart, true);
        }

        #endregion
    }
}

#endif