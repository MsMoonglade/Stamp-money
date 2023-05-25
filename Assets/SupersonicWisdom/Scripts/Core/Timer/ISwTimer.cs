using System;

namespace SupersonicWisdomSDK
{
    internal delegate void OnTickDelegate(float elapsed, float remaining);

    internal interface ISwTimer : ISwTimerListener
    {
        #region --- Properties ---

        bool PauseWhenUnityOutOfFocus { get; set; }
        float Duration { get; set; }
        float Elapsed { get; set; }
        string Name { get; set; }

        #endregion


        #region --- Public Methods ---

        ISwTimer PauseTimer ();
        ISwTimer ResumeTimer ();
        ISwTimer StartTimer ();
        ISwTimer StopTimer ();

        #endregion
    }

    internal interface ISwTimerListener
    {
        #region --- Events ---

        event Action OnFinishEvent;
        event Action OnStoppedEvent;
        event OnTickDelegate OnTickEvent;

        #endregion


        #region --- Properties ---

        bool DidFinish { get; }
        bool IsPaused { get; }
        bool IsReset { get; }

        float Duration { get; }
        ////////////////////////////////////////////////////
        //          // IsEnabled | IsPaused | DidFinished //
        ////////////////////////////////////////////////////
        //  Start   //     V     |    X     |      X      //    
        //--------- // -----------------------------------//
        //  Pause   //     X     |    V     |      X      //
        //--------- // -----------------------------------//
        //  Resume  //     V     |    X     |      X      //
        //--------- // -----------------------------------//
        //   Stop   //     X     |    X     |      X      //
        //--------- // -----------------------------------//
        //  Finish  //     X     |    X     |      V      //
        ////////////////////////////////////////////////////

        float Elapsed { get; }
        bool IsEnabled { get; }

        #endregion
    }
}