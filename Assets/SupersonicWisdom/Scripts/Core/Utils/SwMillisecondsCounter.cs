using System;

namespace SupersonicWisdomSDK
{
    public class SwMillisecondsCounter
    {
        #region --- Members ---

        private long _creationTime;

        #endregion


        #region --- Construction ---

        public SwMillisecondsCounter ()
        {
            Reset();
        }

        #endregion


        #region --- Mono Override ---

        public void Reset ()
        {
            _creationTime = DateTime.Now.SwTimestampMilliseconds();
        }

        #endregion


        #region --- Public Methods ---

        public long Measure ()
        {
            var now = DateTime.Now.SwTimestampMilliseconds();

            return now - _creationTime;
        }

        #endregion
    }
}