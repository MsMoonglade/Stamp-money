using System;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwWebResponse
    {
        #region --- Members ---

        /// <summary>
        ///     Did response resolved (both success/fail)
        /// </summary>
        public bool isDone;

        /// <summary>
        ///     Is response pending
        /// </summary>
        public bool isPending;

        /// <summary>
        ///     Response raw data
        /// </summary>
        [CanBeNull]
        public byte[] data;

        /// <summary>
        ///     Response HTTP Status code
        /// </summary>
        public long code;

        public SwWebRequestError error;
        private bool _didComputeText;
        private string _text;

        #endregion


        #region --- Properties ---

        public bool DidFail
        {
            get { return isDone && error != SwWebRequestError.None; }
        }

        public bool DidSucceed
        {
            get { return isDone && error == SwWebRequestError.None; }
        }

        /// <summary>
        ///     Text is computed from the response data
        ///     Computed only once
        /// </summary>
        [CanBeNull]
        public string Text
        {
            get
            {
                if (!_didComputeText)
                {
                    _text = data != null ? System.Text.Encoding.UTF8.GetString(data, 0, data.Length) : null;
                    _didComputeText = true;
                }

                return _text;
            }
        }

        #endregion
    }
}