using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFunctions : MonoBehaviour
{
    public static UiFunctions instance;
       
    //PRITER SCALE
    private Vector3 printerScale;
    public int printerScaleIndex;

    //JUMP SPEED
    public  float jumpSpeedAddPerLevel;
    public int jumpSpeedIndex;

    //MOVE SPEED
    public float moveSpeedAddPerLevel;
    public int moveSpeedIndex;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.HasKey("PrinterScaleIndex"))
            printerScaleIndex = PlayerPrefs.GetInt("PrinterScaleIndex");
        else
        {
            printerScaleIndex = 1;
            PlayerPrefs.SetInt("PrinterScaleIndex", printerScaleIndex);
        }

        if (PlayerPrefs.HasKey("JumpSpeedIndex"))
            jumpSpeedIndex = PlayerPrefs.GetInt("JumpSpeedIndex");
        else
        {
            jumpSpeedIndex = 0;
            PlayerPrefs.SetInt("JumpSpeedIndex", jumpSpeedIndex);
        }

        if (PlayerPrefs.HasKey("MoveSpeedIndex"))
            moveSpeedIndex = PlayerPrefs.GetInt("MoveSpeedIndex");
        else
        {
            moveSpeedIndex = 0;
            PlayerPrefs.SetInt("MoveSpeedIndex", moveSpeedIndex);
        }
    }

    private void Start()
    {
        printerScale = CharacterBehaviour.instance.printerObject.transform.localScale;  
    }

    public void BuyMoney()
    {
        if (ShopManager.instance.currentGold >= ShopCostHelper.instance.moneyShopCost)
        {
            if (InventoryManager.Instance.HaveFreeSlot())
            {
                ShopManager.instance.SpendCoin(ShopCostHelper.instance.moneyShopCost);

                InventoryManager.Instance.GenerateNewMoney();
            }
        }
    }

    public void IncreasePrinterSize()
    {
        if (ShopManager.instance.currentGold >= ShopCostHelper.instance.sizeIncreaseCost[printerScaleIndex])
        {
            ShopManager.instance.SpendCoin(ShopCostHelper.instance.sizeIncreaseCost[printerScaleIndex]);

            if (printerScaleIndex < 3)
            {
                Vector3 newScale = Vector3.zero;

                if (printerScaleIndex == 1)
                {
                    newScale = printerScale += new Vector3(1.5f, 0, 0);
                    CharacterBehaviour.instance.ApplyPrinterScale(newScale, true);

                }
                else
                {
                    newScale = printerScale += new Vector3(0, 0, 0.75f);
                    CharacterBehaviour.instance.ApplyPrinterScale(newScale, false);
                }

                printerScaleIndex++;
                PlayerPrefs.SetInt("PrinterScaleIndex", printerScaleIndex);
            }
        }
    }

    public void IncreaseFireRate()
    {
        if (ShopManager.instance.currentGold >= ShopCostHelper.instance.actualJumpSpeedCost)
        {
            ShopManager.instance.SpendCoin(ShopCostHelper.instance.actualJumpSpeedCost);

            float amount = jumpSpeedAddPerLevel;
            CharacterBehaviour.instance.IncreaseJumpSpeed(amount);

            jumpSpeedIndex++;
            PlayerPrefs.SetInt("JumpSpeedIndex", jumpSpeedIndex);

            ShopCostHelper.instance.UpdateCost();
        }
    }

    public void IncreaseMoveSpeed()
    {
        if (ShopManager.instance.currentGold >= ShopCostHelper.instance.actualMoveSpeedCost)
        {
            ShopManager.instance.SpendCoin(ShopCostHelper.instance.actualMoveSpeedCost);

            float amount = moveSpeedAddPerLevel;
            CharacterBehaviour.instance.IncreaseMoveSpeed(amount);

            moveSpeedIndex++;
            PlayerPrefs.SetInt("MoveSpeedIndex", moveSpeedIndex);

            ShopCostHelper.instance.UpdateCost();
        }
    }

    public void IncreaseGoldPerHour()
    {        
        PassiveIncome.instance.IncreaseGoldPerHour();        
    }
}
