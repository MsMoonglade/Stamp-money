using DG.Tweening;
using SupersonicWisdomSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
        DOTween.Init();
        // Subscribe
        SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);
        // Then initialize
        SupersonicWisdom.Api.Initialize();
    }

    void OnSupersonicWisdomReady()
    {
        SceneManager.LoadScene(1);
    }
}
