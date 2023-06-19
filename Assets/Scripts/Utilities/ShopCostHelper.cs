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
    public int[] moneyShopCost;
    public int[] sizeIncreaseCost;

    public int actualJumpSpeedCost;
    public int actualMoveSpeedCost;
    public int actualIncomeCost;

    private void Awake()
    {
        instance = this;
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


        actualIncomeCost = startIncomeCost;

        if (PassiveIncome.instance.passiveIncomeIndex >= 1)
        {
            for (int i = 0; i < PassiveIncome.instance.passiveIncomeIndex; i++)
            {
                actualIncomeCost += incomePerHourCostDelta;
            }
        }
    }
}