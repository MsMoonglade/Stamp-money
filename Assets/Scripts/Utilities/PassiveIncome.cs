using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    public static PassiveIncome instance;

    public int[] goldPerHour;
    public GameObject passiveIncomeGo;
    public TMP_Text passiveIncomeText;

    DateTime currentDate;
    DateTime oldDate;

    public int passiveIncomeIndex;

    private int passiveAmmount;

    private void Awake()
    {
        instance = this; 

        passiveAmmount = 0;
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
            int passedHour = CheckOfflineTime();

            passiveAmmount = (goldPerHour[passiveIncomeIndex] * passedHour);    
            passiveIncomeText.text = passiveAmmount.ToString();
        }

        if (passiveAmmount <= 0)
            passiveIncomeGo.gameObject.SetActive(false);
    }

    public int CheckOfflineTime()
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
    public void GetPassiveIncome()
    {
        ShopManager.instance.IncreaseGold(passiveAmmount);
    }
}