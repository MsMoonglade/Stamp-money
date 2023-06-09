﻿using System.Collections;
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

    public Image energySlider;

    public TMP_Text levelText;
    public TMP_Text endLevelText;

    [Header("InstantiateElement")]
    public GameObject ui_Coin_Prefs;
    public GameObject ui_Coin_Destination;
    public float ui_Coin_AnimSpeed;

    public GameObject ui_Diamond_Prefs;
    public GameObject ui_Diamond_Destination;
    public float ui_Diamond_AnimSpeed;

    public GameObject ui_Energy_Prefs;
    public GameObject ui_Energy_Destination;
    public float ui_Energy_AnimSpeed;

    private Image[] energySliderImages;

    public GameObject uiTempParent;

    private void Awake()
    {
        instance = this;

        energySliderImages = new Image[energySlider.transform.parent.transform.childCount];
        for(int i = 0; i < energySlider.transform.parent.transform.childCount; i++)
        {
            energySliderImages[i] = energySlider.transform.parent.transform.GetChild(i).GetComponent<Image>();
        }

        foreach (Image image in energySliderImages)
        {
            image.DOFade(0, 0.05f);
        }

        //     DisableAllUi();
        //     EnableMainMenuUi();
    }

    private void Start()
    {
        energySlider.fillAmount = CharacterBehaviour.instance.currentEnergy / CharacterBehaviour.instance.maxEnergy;

        retryUi.gameObject.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (GameManager.instance.IsInGameStatus())
        {
            energySlider.fillAmount = CharacterBehaviour.instance.currentEnergy / CharacterBehaviour.instance.maxEnergy;
        }
    }

    public void ShowEnergyslider()
    {
        foreach (Image image in energySliderImages)
        {
            image.DOFade(1, 0.25f);
        }
    }

    public void HideEnergySlider()
    {
        foreach (Image image in energySliderImages)
        {
            image.DOFade(0, 0.25f);
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

    public void DisableGameUI()
    {
        gameUi.SetActive(false);
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

        retryUi.transform.DOScale(Vector3.one, 0.5f);
    }

    public void DisableAllUi()
    {
        mainMenuUi.SetActive(false);
        endGameUi.SetActive(false);
        gameUi.SetActive(false);
    }

    public void InstantiateCoin(int amount , GameObject localPosToRef)
    {
        for(int i = 0; i < amount; i++)
        {
            Vector3 posToRef = localPosToRef.transform.position;
            
            if(localPosToRef == CharacterBehaviour.instance.gameObject)           
                posToRef += new Vector3(Random.Range(-4.0f, 4.0f),0, Random.Range(-4.0f, 8.0f));
            else
                posToRef += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));



            Vector3 pos =  Camera.main.WorldToScreenPoint(posToRef);

            GameObject coin = Instantiate(ui_Coin_Prefs, pos, Quaternion.identity, uiTempParent.transform);
            coin.transform.localScale = Vector3.zero;

            coin.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);

            float animSpeed = Random.Range(ui_Coin_AnimSpeed - 0.2f, ui_Coin_AnimSpeed + 0.2f);
            coin.transform.DOMove(ui_Coin_Destination.transform.position, animSpeed)
                .SetEase(Ease.InBack)             
                .OnComplete(() => Destroy(coin));       
        }
    }

    public void InstantiateEnergy(float energyToAdd , int amount , GameObject localPosition)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 posToRef = localPosition.transform.position;
            posToRef += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            Vector3 pos = Camera.main.WorldToScreenPoint(posToRef);

            GameObject energy = Instantiate(ui_Energy_Prefs, pos, Quaternion.identity, uiTempParent.transform);
            energy.transform.localScale = Vector3.zero;

            energy.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);

            float animSpeed = Random.Range(ui_Energy_AnimSpeed - 0.2f, ui_Energy_AnimSpeed + 0.2f);
            float ePerElementAmount = energyToAdd / amount;

            energy.transform.DOMove(ui_Energy_Destination.transform.position, animSpeed)
                .SetEase(Ease.InBack)
                .OnComplete(() => EnergyAddCallback(ePerElementAmount , energy));
        }
    }

    private void EnergyAddCallback(float eAmount , GameObject o)
    {
        CharacterBehaviour.instance.IncreaseEnergy(eAmount);
        Destroy(o);
    }

    public void LostCoin(int amount)
    {
        for (int i = 0; i < amount; i++)
        {            
            GameObject coin = Instantiate(ui_Coin_Prefs, ui_Coin_Destination.transform.position, Quaternion.identity, uiTempParent.transform);

            float animSpeed = Random.Range(ui_Coin_AnimSpeed - 0.1f, ui_Coin_AnimSpeed + 0.1f);
            Vector3 destination = new Vector3(coin.transform.position.x - Random.Range(-50, 20), coin.transform.position.y - 3000, 0);

            coin.transform.DOMove(destination, animSpeed)                
                .OnComplete(() => Destroy(coin));
        }
    }

    public void InstantiateDiamond(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 posToRef = CharacterBehaviour.instance.transform.position;
            posToRef += new Vector3(Random.Range(-4.0f, 4.0f), 0, Random.Range(-4.0f, 8.0f));

            Vector3 pos = Camera.main.WorldToScreenPoint(posToRef);

            GameObject coin = Instantiate(ui_Diamond_Prefs, pos, Quaternion.identity, uiTempParent.transform);
            coin.transform.localScale = Vector3.zero;

            coin.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);

            float animSpeed = Random.Range(ui_Diamond_AnimSpeed - 0.1f, ui_Diamond_AnimSpeed + 0.1f);
            coin.transform.DOMove(ui_Diamond_Destination.transform.position, animSpeed)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(coin));
        }
    }

    public void InstantiateDiamond(int amount, GameObject localPosition)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 posToRef = localPosition.transform.position;
            posToRef += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            Vector3 pos = Camera.main.WorldToScreenPoint(posToRef);

            GameObject coin = Instantiate(ui_Diamond_Prefs, pos, Quaternion.identity, uiTempParent.transform);
            coin.transform.localScale = Vector3.zero;

            coin.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);

            float animSpeed = Random.Range(ui_Diamond_AnimSpeed - 0.1f, ui_Diamond_AnimSpeed + 0.1f);
            coin.transform.DOMove(ui_Diamond_Destination.transform.position, animSpeed)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(coin));
        }
    }
}