using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGameConsoleMono : MonoBehaviour
    {
        #region --- Constants ---

        private const string CloseConsoleButtonText = "Close";
        private const string OpenConsoleButtonText = "Open";

        #endregion


        #region --- Members ---

        private static SwGameConsoleMono _instance;
        private bool _closeButtonPresent = true;
        private float _customScreenHeight;
        private float _customScreenWidth;
        private GUIStyle _guiCloseOpenConsoleButtonStyle;
        private GUIStyle _guiLogConsoleStyle;
        private int _lineNumber;
        private StringBuilder _consoleText;
        private Vector2 _scrollPosition = Vector2.zero;

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            //Once created, we make the gameObject persistant
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);

                return;
            }

            CreateConsole();
            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI ()
        {
            if (_closeButtonPresent)
            {
                ShowButton(CloseConsoleButtonText);
                RenderConsoleWindow();
            }
            else
            {
                ShowButton(OpenConsoleButtonText);
            }
        }

        private void Start ()
        {
            //Sets console dimensions based on the device screen size
            _customScreenHeight = Screen.height * 0.33f;
            _customScreenWidth = Screen.width;
        }

        #endregion


        #region --- Public Methods ---

        public void HideConsole ()
        {
            _closeButtonPresent = false;
        }

        public void InternalLogToConsole(string log, string stack, string logType)
        {
            if (!_closeButtonPresent) StartCoroutine(nameof(FlashButtonText));

            var logColor = LogLevelToColor(logType);

            _lineNumber += 1;
            _consoleText.Insert(0, $"{_lineNumber}: <color={logColor}>{log}</color> {Environment.NewLine} {stack}");
        }

        #endregion


        #region --- Private Methods ---

        private void CreateConsole ()
        {
            //GUI template, controls the look and feel of the console

            _guiLogConsoleStyle = new GUIStyle
            {
                normal = { background = Texture2D.grayTexture },
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                border = new RectOffset(1, 1, 1, 1),
                padding = new RectOffset(25, 25, 25, 25),
                margin = new RectOffset(25, 25, 75, 25),
                stretchHeight = true
            };

            _guiCloseOpenConsoleButtonStyle = new GUIStyle("button");
            _guiCloseOpenConsoleButtonStyle.fontSize = 42;
            _guiCloseOpenConsoleButtonStyle.alignment = TextAnchor.MiddleCenter;
            _guiCloseOpenConsoleButtonStyle.normal.background = Texture2D.grayTexture;

            _consoleText = new StringBuilder(1024);
        }

        private IEnumerator FlashButtonText ()
        {
            //Small visual effect to alert the user that a new log was added 
            _guiCloseOpenConsoleButtonStyle.fontSize = 0;

            while (_guiCloseOpenConsoleButtonStyle.fontSize < 42)
            {
                _guiCloseOpenConsoleButtonStyle.fontSize += 1;

                yield return new WaitForEndOfFrame();
            }
        }

        private string LogLevelToColor(string logLevel)
        {
            string color;

            switch (logLevel)
            {
                case "Warning":
                    color = "yellow";

                    break;
                case "Error":
                    color = "orange";

                    break;
                case "Exception":
                    color = "red";

                    break;
                default:
                    color = "white";

                    break;
            }

            return color;
        }

        private void RenderConsoleWindow ()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(_customScreenWidth), GUILayout.Height(_customScreenHeight));
            GUILayout.Label(_consoleText.ToString(), _guiLogConsoleStyle);
            GUILayout.EndScrollView();
        }

        private void ShowButton(string text)
        {
            if (GUI.Button(new Rect(5f, _closeButtonPresent ? _customScreenHeight : 1, 250f, 100f), text, _guiCloseOpenConsoleButtonStyle))
            {
                ToggleButtonState();
            }
        }

        private void ToggleButtonState ()
        {
            _closeButtonPresent = !_closeButtonPresent;
        }

        #endregion
    }
}