using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCostHelper : MonoBehaviour
{
    public static ShopCostHelper instance;

    public int[] jumpSpeedCost;
    public int[] incomePerHourCost;
    public int[] moneyShopCost;
    public int[] sizeIncreaseCost;

    private void Awake()
    {
        instance = this;
    }
}