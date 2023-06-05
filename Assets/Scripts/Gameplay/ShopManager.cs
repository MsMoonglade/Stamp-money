using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    
    public int currentGold;
    public int currentDiamond = 0;

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
        currentGold += amount;

        UiManager.instance.InstantiateCoin(amount);
    }

    public void IncreaseDiamond(int amount)
    {
        currentDiamond += amount;
        UiManager.instance.InstantiateDiamond(amount);
    }

    private void OnSaveValue(object sender)
    {
        currentGold = 0;
        PlayerPrefs.SetInt("GoldCurrency", currentGold);
    }
}