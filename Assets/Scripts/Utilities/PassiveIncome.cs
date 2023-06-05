using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    public static PassiveIncome instance;

    public int[] goldPerHour;

    DateTime currentDate;
    DateTime oldDate;

    public int passiveIncomeIndex;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PassiveIncomeIndex"))
            passiveIncomeIndex = PlayerPrefs.GetInt("PassiveIncomeIndex");
        else
        {
            passiveIncomeIndex = 0;
            PlayerPrefs.SetInt("PassiveIncomeIndex", passiveIncomeIndex);
        }
        
        if (PlayerPrefs.HasKey("OldHour"))
        {
            int passedHour = CheckOfflinetime();

            int amount = (goldPerHour[passiveIncomeIndex] * passedHour);
            ShopManager.instance.IncreaseGold(amount);
        }
    }

    public int CheckOfflinetime()
    {
        string savedTime = PlayerPrefs.GetString("OldHour");
                
        oldDate = System.DateTime.Parse(savedTime);
        currentDate = System.DateTime.Now;

        TimeSpan difference = currentDate.Subtract(oldDate);

        return (int)difference.TotalHours;        
    }

    public void IncreaseGoldPerHour()
    {
        if(passiveIncomeIndex < goldPerHour.Length -1)
        {
            passiveIncomeIndex ++;
            PlayerPrefs.SetInt("PassiveIncomeIndex", passiveIncomeIndex);
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("OldHour", System.DateTime.Now.ToString());
    }
}