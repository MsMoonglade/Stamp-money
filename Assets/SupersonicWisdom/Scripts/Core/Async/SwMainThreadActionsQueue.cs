using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal class SwMainThreadActionsQueue
    {
        #region --- Members ---

        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        #endregion


        #region --- Public Methods ---

        public void Add([NotNull] Action action)
        {
            _actions.Enqueue(action);
        }

        public void Run ()
        {
            while (_actions.TryDequeue(out var action))
            {
                try
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
                catch (Exception e)
                {
                    SwInfra.Logger?.LogError($"SwMainThreadActionsQueue | Run | Error: {e.Message}");
                }
            }
        }

        #endregion
    }
}