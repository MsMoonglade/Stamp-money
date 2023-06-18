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

    public void IncreaseGold(int amount , GameObject localPosToRef)
    {
        if (amount > 0)
        {
            currentGold += amount;

            currentGoldText.text = currentGold.ToString();

            UiManager.instance.InstantiateCoin(amount , localPosToRef);
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
    public void IncreaseGold(int amount, GameObject localPosToRef, bool endGameWall)
    {
        currentGold += amount;
        currentGoldText.text = currentGold.ToString();

        StartCoroutine(AnimEndGameWallCoin((int) (amount / 10) , localPosToRef));
    }

    private IEnumerator AnimEndGameWallCoin(int amount , GameObject pos)
    {
        for(int i = 0; i < amount; i++)
        {
            UiManager.instance.InstantiateCoin(1, pos);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void IncreaseGoldGM()
    {
        currentGold += 1000;
        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }

    public void IncreaseDiamond(int amount)
    {
        currentDiamond += amount;

        currentDiamondText.text = currentDiamond.ToString();

        UiManager.instance.InstantiateDiamond(amount);
    }

    public void IncreaseDiamond(int amount , GameObject pos)
    {
        currentDiamond += amount;

        currentDiamondText.text = currentDiamond.ToString();

        UiManager.instance.InstantiateDiamond(amount , pos);
    }

    public void IncreaseEnergy(float amount , GameObject pos)
    {
        CharacterBehaviour.instance.currentEnergy += amount;

        int uitoInstantiate = (int)(amount / 0.25f);

        UiManager.instance.InstantiateEnergy(uitoInstantiate , pos);
    }

    public void DecreaseDiamond(int amount)
    {
        currentDiamond -= amount;

        currentDiamondText.text = currentDiamond.ToString();
    }

    public void SpendCoin(int amount)
    {
        currentGold -= amount;

        if (currentGold <= 0)
            currentGold = 0;

        currentGoldText.text = currentGold.ToString();
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