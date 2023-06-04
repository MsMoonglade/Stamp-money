using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;
using DG.Tweening;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public GameObject mainMenuUi;
    public GameObject gameUi;
    public GameObject endGameUi;
    public GameObject retryUi;

    public Slider energySlider;

    public TMP_Text levelText;
    public TMP_Text endLevelText;

    public MMF_Player enableRetryFeedback;
    public MMF_Player disableRetryFeedback;
    public MMF_Player enableEndGameFeedback;
    public MMF_Player disableEndGameFeedback;

    private void Awake()
    {
        instance = this;

        //     DisableAllUi();
        //     EnableMainMenuUi();

        disableRetryFeedback.PlayFeedbacks();
        disableEndGameFeedback.PlayFeedbacks(); 
    }

    private void Start()
    {
        energySlider.value = CharacterBehaviour.instance.currentEnergy / CharacterBehaviour.instance.maxEnergy;
    }

    private void Update()
    {
        if (GameManager.instance.IsInGameStatus())
        {
            energySlider.value = CharacterBehaviour.instance.currentEnergy / CharacterBehaviour.instance.maxEnergy;
        }
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
        //endGameUi.SetActive(false);
        mainMenuUi.SetActive(false);
        gameUi.SetActive(true);

        AnimateUiElement.instance.HideAll();
    }

    public void EnableEndGameUi()
    {
        mainMenuUi.SetActive(false);
        gameUi.SetActive(false);
        endGameUi.SetActive(true);
        enableEndGameFeedback.PlayFeedbacks();
    }

    public void EnableRetryUi()
    {        
        mainMenuUi.SetActive(false);
        gameUi.SetActive(false);
        endGameUi.SetActive(false);
        retryUi.SetActive(true);
        enableRetryFeedback.PlayFeedbacks();
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