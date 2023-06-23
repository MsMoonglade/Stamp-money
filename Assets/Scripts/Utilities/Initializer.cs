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
        CheckPlayerPrefsNewBuild();
        
        // Subscribe
        SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);
        // Then initialize
        SupersonicWisdom.Api.Initialize();
    }

    void OnSupersonicWisdomReady()
    {
        SceneManager.LoadScene(1);
    }

    void CheckPlayerPrefsNewBuild()
    {
        if (PlayerPrefs.HasKey("Build_1.1.2"))
            return;
        else
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("Build_1.1.2", 1);
        }
    }
}
