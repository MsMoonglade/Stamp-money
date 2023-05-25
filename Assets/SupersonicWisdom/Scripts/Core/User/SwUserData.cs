using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwUserData : ISwUserData
    {
        #region --- Constants ---

        private const string UserStateStorageKey = "SupersonicWisdomUserState";

        #endregion


        #region --- Events ---

        internal delegate void OnUserStateChange(SwUserState newState, SwUserStateChangeReason reason);

        internal event OnUserStateChange OnUserStateChangeEvent;

        #endregion


        #region --- Members ---

        public readonly string BundleIdentifier;

        public readonly string Language;
        public readonly string Platform;
        public string Country;

        protected readonly ISwSettings Settings;
        protected readonly SwStoreKeys StoreKeys;

        private readonly ISwAdvertisingIdsGetter _idsGetter;
        private long _installDateInSeconds;
        private string _customUuid;

        private string _installDate;

        private SwUserState _userState;

        #endregion


        #region --- Properties ---

        public string OrganizationAdvertisingId { get; private set; }

        public string Uuid { get; private set; }

        /// <summary>
        ///     Indicates if this is a new running app session (not the user's session that affected by foreground / background
        ///     changes).
        /// </summary>
        public bool IsNew { private set; get; }

        public DateTime InstallDateTime { private set; get; }

        public long InstallDateInSeconds
        {
            get { return _installDateInSeconds; }
            private set
            {
                _installDateInSeconds = value;
                SwInfra.KeyValueStore.SetString(StoreKeys.InstallTime, _installDateInSeconds.ToString());
                SwInfra.KeyValueStore.Save();
            }
        }

        public string CustomUuid
        {
            get { return _customUuid; }
            private set
            {
                _customUuid = value;
                SwInfra.KeyValueStore.SetString(StoreKeys.CustomUuid, _customUuid);
                SwInfra.KeyValueStore.Save();
            }
        }

        public string InstallDate
        {
            get { return _installDate; }
            private set
            {
                _installDate = value;
                SwInfra.KeyValueStore.SetString(StoreKeys.InstallDate, _installDate);
                SwInfra.KeyValueStore.Save();
            }
        }

        #endregion


        #region --- Construction ---

        public SwUserData(ISwSettings settings, SwStoreKeys storeKeys, ISwAdvertisingIdsGetter idsGetter)
        {
            Settings = settings;
            StoreKeys = storeKeys;
            _idsGetter = idsGetter;

            Language = SwUtils.GetSystemLanguageIso6391();
            Country = SwUtils.GetCountry();
            BundleIdentifier = Application.identifier;
            Platform = Application.platform.ToString();
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Load(ISwInitParams initParams)
        {
            LoadUuid();
            LoadInstallDate();
            LoadCustomUuid();
            LoadUserState();
        }

        public long GetSecondsFromInstall ()
        {
            if (InstallDateInSeconds == 0)
            {
                return -1;
            }

            return SwUtils.GetTotalSeconds(DateTime.UtcNow) - InstallDateInSeconds;
        }

        public void LoadUuid ()
        {
            var advertisingId = _idsGetter.GetAdvertisingId();
            var organizationAdvertisingId = _idsGetter.GetOrganizationAdvertisingId();

            SwInfra.Logger.Log($"Got advertising ID ('{advertisingId}') and organization advertising ID ('{organizationAdvertisingId}')");

            Uuid = advertisingId;
            OrganizationAdvertisingId = organizationAdvertisingId;
        }

        public bool ModifyUserStateSync(Action<SwUserState> modifier)
        {
            return ModifyUserStateSync(s =>
            {
                modifier.Invoke(s);

                return true;
            });
        }

        public bool ModifyUserStateSync(Func<SwUserState, bool> modifier)
        {
            var copyOfUserState = _userState.Copy();
            var didChange = modifier.Invoke(copyOfUserState);

            if (didChange)
            {
                _userState = copyOfUserState;
                PersistUserState();
            }

            return didChange;
        }

        public bool UpdateAge(SwUserState userState)
        {
            var didChange = false;
            var currentAge = CalculateCurrentAge();

            if (currentAge != userState.age)
            {
                userState.age = currentAge;
                userState.todaySessionsCount = 0;
                didChange = true;
            }

            return didChange;
        }

        #endregion


        #region --- Private Methods ---

        [NotNull]
        protected virtual SwUserState DeserializeUserState(string userStateString)
        {
            return JsonUtility.FromJson<SwUserState>(userStateString) ?? new SwUserState();
        }

        // TODO Perry: We might want to include this logic inside the `ModifyUserStateSync` later, but currently we avoid it due to task complexity.
        internal void AfterUserStateChangeInternal(SwUserStateChangeReason reason, bool silent = false)
        {
            if (!silent)
            {
                OnUserStateChangeEvent?.Invoke(ImmutableUserState(), reason);
            }
        }

        internal SwUserState ImmutableUserState ()
        {
            return _userState.Copy();
        }

        private long CalculateCurrentAge ()
        {
            return Convert.ToInt64((DateTime.UtcNow - InstallDateTime).Days);
        }

        private void InjectTestDataToUserState([NotNull] SwUserState userStateValue)
        {
            if (SwTestUtils.CustomUserState != null)
            {
                if (SwTestUtils.CustomUserState.ContainsKey("todaySessionsCount"))
                {
                    userStateValue.todaySessionsCount = (long)SwTestUtils.CustomUserState["todaySessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("totalSessionsCount"))
                {
                    userStateValue.totalSessionsCount = (long)SwTestUtils.CustomUserState["totalSessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("completedLevels"))
                {
                    userStateValue.completedLevels = (long)SwTestUtils.CustomUserState["completedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("playedLevels"))
                {
                    userStateValue.playedLevels = (long)SwTestUtils.CustomUserState["playedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveFailedLevels"))
                {
                    userStateValue.consecutiveFailedLevels = (long)SwTestUtils.CustomUserState["consecutiveFailedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveCompletedLevels"))
                {
                    userStateValue.consecutiveCompletedLevels = (long)SwTestUtils.CustomUserState["consecutiveCompletedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("isDuringLevel"))
                {
                    userStateValue.isDuringLevel = (bool)SwTestUtils.CustomUserState["isDuringLevel"];
                }
            }
        }

        private void LoadCustomUuid ()
        {
            _customUuid = SwInfra.KeyValueStore.GetString(StoreKeys.CustomUuid);

            if (string.IsNullOrEmpty(_customUuid))
            {
                CustomUuid = Guid.NewGuid().ToString();
            }
        }

        private void LoadInstallDate ()
        {
            var dateInSecondsFromPrefs = SwInfra.KeyValueStore.GetString(StoreKeys.InstallTime);
            var dateFromPrefs = SwInfra.KeyValueStore.GetString(StoreKeys.InstallDate);

            if (!string.IsNullOrEmpty(SwTestUtils.CustomInstallDate))
            {
                InstallDate = SwTestUtils.CustomInstallDate;
            }

            if (string.IsNullOrEmpty(InstallDate) && !string.IsNullOrEmpty(dateFromPrefs))
            {
                InstallDate = dateFromPrefs;
            }

            if (!string.IsNullOrEmpty(dateInSecondsFromPrefs))
            {
                try
                {
                    InstallDateInSeconds = Convert.ToInt64(dateInSecondsFromPrefs);
                }
                catch (Exception)
                {
                    SwInfra.Logger.LogError("Cannot parse install in seconds from PlayerPerfs");
                }
            }

            // User is considered new if there are no saved values by wisdom
            IsNew = string.IsNullOrEmpty(InstallDate) && !SwInfra.KeyValueStore.HasKey(StoreKeys.Config);

            if (string.IsNullOrEmpty(InstallDate))
            {
                var date = DateTime.UtcNow;

                InstallDate = date.SwToString("yyyy-MM-dd");

                var seconds = SwUtils.GetTotalSeconds(date);

                if (InstallDateInSeconds == 0)
                {
                    InstallDateInSeconds = seconds;
                }
            }

            var installDateComponents = InstallDate.Split('-');
            InstallDateTime = new DateTime(Convert.ToInt32(installDateComponents[0]), Convert.ToInt32(installDateComponents[1]), Convert.ToInt32(installDateComponents[2]), 0, 0, 0, DateTimeKind.Utc);
        }

        private void LoadUserState ()
        {
            var userStateString = SwInfra.KeyValueStore.GetString(UserStateStorageKey, "{}");
            var userStateValue = DeserializeUserState(userStateString);

            InjectTestDataToUserState(userStateValue);
            UpdateAge(userStateValue);
            _userState = userStateValue.Copy();
        }

        private void PersistUserState ()
        {
            SwInfra.KeyValueStore.SetString(UserStateStorageKey, JsonUtility.ToJson(ImmutableUserState()));
        }

        #endregion
    }
}