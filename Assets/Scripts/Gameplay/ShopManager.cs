using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    
    public int currentGold;
    public int currentDiamond = 0;

    public TMP_Text currentGoldText;
    public TMP_Text currentDiamondText;

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

    private void OnEnable()
    {
        EventManager.StartListening(Events.endGame, OnSaveValue);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.endGame, OnSaveValue);
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

    //must be called in gamemanager
    private void OnSaveValue(object sender)
    {
        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }
}