using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwCoreMonoBehaviour : MonoBehaviour, ISwMainThreadRunner
    {
        #region --- Members ---

        private static bool _hasInstance;

        private readonly SwMainThreadActionsQueue _mainThreadActionsQueue = new SwMainThreadActionsQueue();
        private ISwScriptLifecycleListener _lifecycleListener;

        #endregion


        #region --- Properties ---

        internal ISwScriptLifecycleListener LifecycleListener
        {
            set { _lifecycleListener = value; }
            get
            {
                // Make sure that even if the game developer manually adds our Prefab - the SDK flow won't get compromise.
                if (!(_lifecycleListener is ISwContainer))
                {
                    throw new SwException("SupersonicWisdom's main game object wasn't initialized properly! It should be added automatically after calling `SupersonicWisdom.Api.Initialize();`. Did you accidentally add it manually to one of your scenes? If so, please remove it.");
                }

                return _lifecycleListener;
            }
        }

        #endregion


        #region --- Public Methods ---

        public new T GetComponent<T> ()
        {
            var component = base.GetComponent<T>();

            if (component == null)
            {
                throw new Exception($"{this} Does not have a component of type {typeof(T).FullName}");
            }

            return component;
        }

        /// <summary>
        ///     Run actions on main thread.
        ///     A notable caveat is that you can use it only when Unity is not in focus.
        ///     Otherwise, it can crash the app due to a lock applied on Main Thread
        /// </summary>
        /// <param name="action"></param>
        public void RunOnMainThread(Action action)
        {
            if (SwUtils.IsRunningOnMainThread)
            {
                action.Invoke();

                return;
            }

            _mainThreadActionsQueue.Add(action);
        }

        #endregion


        #region MonoBehavior Implementation

        private void Awake ()
        {
            if (_hasInstance)
            {
                Destroy(gameObject);

                return;
            }

            _hasInstance = true;

            DontDestroyOnLoad(gameObject);
            LifecycleListener.OnAwake();
        }

        private void Start ()
        {
            if (_hasInstance) LifecycleListener.OnStart();
        }

        private void Update ()
        {
            if (_hasInstance)
            {
                _mainThreadActionsQueue.Run();
                LifecycleListener.OnUpdate();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_hasInstance) LifecycleListener.OnApplicationPause(pauseStatus);
        }

        private void OnApplicationQuit ()
        {
            if (_hasInstance) LifecycleListener.OnApplicationQuit();
        }

        private void OnDestroy ()
        {
            _hasInstance = false;
        }

        #endregion
    }
}