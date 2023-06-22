using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopCostHelper : MonoBehaviour
{
    public static ShopCostHelper instance;

    public int startJumpSpeedCost;
    public int startMoveSpeedCost;
    public int startIncomeCost;

    public int jumpSpeedCostDelta;
    public int moveSpeedCostDelta;
    public int incomePerHourCostDelta;
    public int moneyShopCost;
    public int[] sizeIncreaseCost;

    public int moneyBaseCost;
    public int money4Cost;
    public int money8Cost;
    public UiButtonCostRefresh buttonToRefresh;


    public int actualJumpSpeedCost;
    public int actualMoveSpeedCost;
    public int actualIncomeCost;

    private int moneyUnlockedIndex;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("MoneyUnlockedIndex"))
            moneyUnlockedIndex = PlayerPrefs.GetInt("MoneyUnlockedIndex");
        else
        {
            moneyUnlockedIndex = 0;
            PlayerPrefs.SetInt("MoneyUnlockedIndex", moneyUnlockedIndex);            
        }

        SetMoneyCost();
    }

    private void Start()
    {
       UpdateCost();
    }

    public void UpdateCost()
    {
        actualJumpSpeedCost = startJumpSpeedCost;

        if (UiFunctions.instance.jumpSpeedIndex >= 1)
        {
            for (int i = 0; i < UiFunctions.instance.jumpSpeedIndex; i++)
            {
                actualJumpSpeedCost += jumpSpeedCostDelta;
            }
        }

        actualMoveSpeedCost = startMoveSpeedCost;

        if (UiFunctions.instance.moveSpeedIndex >= 1)
        {
            for (int i = 0; i < UiFunctions.instance.moveSpeedIndex; i++)
            {
                actualMoveSpeedCost += moveSpeedCostDelta;
            }
        }

        actualIncomeCost = startIncomeCost;

        if (PassiveIncome.instance.passiveIncomeIndex >= 1)
        {
            for (int i = 0; i < PassiveIncome.instance.passiveIncomeIndex; i++)
            {
                actualIncomeCost += incomePerHourCostDelta;
            }
        }
    }

    public void SetMoneyCost()
    {
        switch (moneyUnlockedIndex)
        {
            case 0:
                moneyShopCost = moneyBaseCost;
                break;
            case 1:
                moneyShopCost = money4Cost;
                break;
            case 2:
                moneyShopCost = money8Cost;
                break;
        }
    }

    public void Money4Unlocked()
    {
        if (moneyUnlockedIndex == 0)
        {
            moneyUnlockedIndex = 1;
            PlayerPrefs.SetInt("MoneyUnlockedIndex", moneyUnlockedIndex);

            SetMoneyCost();
        }

        buttonToRefresh.CheckValue();
    }

    public void Money8Unlocked()
    {
        if (moneyUnlockedIndex == 0 || moneyUnlockedIndex == 1)
        {
            moneyUnlockedIndex = 2;
            PlayerPrefs.SetInt("MoneyUnlockedIndex", moneyUnlockedIndex);

            SetMoneyCost();
        }

        buttonToRefresh.CheckValue();
    }
}