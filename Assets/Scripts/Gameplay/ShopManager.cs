using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    
    public int currentGold;
    public int currentDiamond = 0;

    public TMP_Text currentGoldText;
    public TMP_Text currentDiamondText;
    public Image goldImage;
    public Image diamondImage;

    public CinemachineVirtualCamera virtualCamera;
    private float shakeTime;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("GoldCurrency"))
            currentGold = PlayerPrefs.GetInt("GoldCurrency");
        else
        {
            currentGold = 0;
            PlayerPrefs.SetInt("GoldCurrency", currentGold);
        }

        currentGoldText.text = currentGold.ToString();
        currentDiamondText.text = currentDiamond.ToString();
    }

    private void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            if(shakeTime <= 0)
            {
                virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            }
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening(Events.saveValue, OnSaveValue);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.saveValue, OnSaveValue);
    }

    public void IncreaseGold(int amount)
    {
        if (amount > 0)
        {
            currentGold += amount;

            currentGoldText.text = currentGold.ToString();

            UiManager.instance.InstantiateCoin(amount);
        }
        else
        {
            int value;

            if (Mathf.Abs(amount) > currentGold)
                value = currentGold;
            else
                value = Mathf.Abs(amount);

            currentGold -= value;
            StartShake();

            if (currentGold <= 0)
                currentGold = 0;
            
            currentGoldText.text = currentGold.ToString();

            UiManager.instance.LostCoin(value);
        }
    }
    
    public void IncreaseDiamond(int amount)
    {
        currentDiamond += amount;

        currentDiamondText.text = currentDiamond.ToString();

        UiManager.instance.InstantiateDiamond(amount);
    }

    public void DecreaseDiamond(int amount)
    {
        currentDiamond -= amount;

        currentDiamondText.text = currentDiamond.ToString();
    }

    public void TweenDiamondUi()
    {
        currentDiamondText.transform.DOScale(1.15f, 0.07f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);

        diamondImage.transform.DOScale(1.15f, 0.07f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);
    }

    public void TweenGoldUi()
    {
        currentGoldText.transform.DOScale(1.15f, 0.07f)     
            .SetEase(Ease.Linear)     
            .SetLoops(2, LoopType.Yoyo);

        goldImage.transform.DOScale(1.15f, 0.07f)
            .SetEase(Ease.Linear)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void StartShake()
    {
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2;
        shakeTime = 0.5f;
    }

    //must be called in gamemanager
    private void OnSaveValue(object sender)
    {
        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }
}