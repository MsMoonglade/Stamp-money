using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject mainMenuUi;
    public GameObject gameUi;
    public GameObject endGameUi;
    public GameObject retryUi;

    private void Awake()
    {
        instance = this;

        //     DisableAllUi();
        //     EnableMainMenuUi();
    }

    public void EnableMainMenuUi()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

        /*
        gameUi.SetActive(false);
        endGameUi.SetActive(false);
        restartUi.SetActive(false);
        mainMenuUi.SetActive(true);
        */
    }

    public void EnableGameUi()
    {
        endGameUi.SetActive(false);
        mainMenuUi.SetActive(false);
        gameUi.SetActive(true);
    }

    public void EnableEndGameUi()
    {
        mainMenuUi.SetActive(false);
        gameUi.SetActive(false);
        endGameUi.SetActive(true);
    }

    public void EnableRetryUi()
    {        
        mainMenuUi.SetActive(false);
        gameUi.SetActive(false);
        endGameUi.SetActive(false);
        retryUi.SetActive(true);
    }

    public void DisableAllUi()
    {
        mainMenuUi.SetActive(false);
        endGameUi.SetActive(false);
        gameUi.SetActive(false);
    }

    public void UpdateEndLevelUi()
    {

    }
}