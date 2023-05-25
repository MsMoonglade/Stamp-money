namespace SupersonicWisdomSDK
{
    internal interface ISwUserStateListener
    {
        #region --- Public Methods ---

        void OnUserStateChange(SwUserState newState, SwUserStateChangeReason reason);

        #endregion
    }
}