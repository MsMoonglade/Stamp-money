using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    public static PassiveIncome instance;

    public int goldPerHourStartValue;

    public int goldPerHourIncrementDelta;
    public GameObject passiveIncomeGo;
    public TMP_Text passiveIncomeText;

    DateTime currentDate;
    DateTime oldDate;

    public int passiveIncomeIndex;

    private int passiveAmmount;

    public int actualGoldPerHour;

    private void Awake()
    {
        instance = this; 

        passiveAmmount = 0;

        if (PlayerPrefs.HasKey("PassiveIncomeIndex"))
            passiveIncomeIndex = PlayerPrefs.GetInt("PassiveIncomeIndex");
        else
        {
            passiveIncomeIndex = 0;
            PlayerPrefs.SetInt("PassiveIncomeIndex", passiveIncomeIndex);
        }

        CheckActualGold();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("OldHour"))
        {
            int passedHour = CheckOfflineTime();

            int index = passiveIncomeIndex;
            
            if (index >= 4)
            {
                index = 3;
            }            

            passiveAmmount = (actualGoldPerHour * passedHour);    
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
        if (ShopManager.instance.currentGold >= ShopCostHelper.instance.actualIncomeCost)
        {
            ShopManager.instance.SpendCoin(ShopCostHelper.instance.actualIncomeCost);


            passiveIncomeIndex++;
            PlayerPrefs.SetInt("PassiveIncomeIndex", passiveIncomeIndex);

            CheckActualGold();
            ShopCostHelper.instance.UpdateCost();
        }
    }

    void CheckActualGold()
    {
        actualGoldPerHour = goldPerHourStartValue;
       
        if (passiveIncomeIndex >= 1)
        {
            for (int i = 0; i < passiveIncomeIndex; i++)
            {
                actualGoldPerHour += goldPerHourIncrementDelta;
            }
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("OldHour", System.DateTime.Now.ToString());
    }
    public void GetPassiveIncome()
    {
        ShopManager.instance.IncreaseGold(passiveAmmount , CharacterBehaviour.instance.gameObject);
        EventManager.TriggerEvent(Events.saveValue);
    }
}