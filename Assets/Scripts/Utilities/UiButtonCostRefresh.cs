using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonCostRefresh : MonoBehaviour
{
    public TMP_Text costText;
    public TMP_Text valueText;

    public ButtonType type;
    
    public Color greenColor;
    public Color redColor;

    List<UiButtonCostRefresh> allButtons = new List<UiButtonCostRefresh>();

    private bool haveChecked;

    private void Start()
    {
        var allB = FindObjectsOfType<UiButtonCostRefresh>();

        for(int i = 0; i < allB.Length; i++)
        {
            allButtons.Add(allB[i]);
        }

        foreach (UiButtonCostRefresh button in allButtons)
        {
            button.CheckValue();
        }

        haveChecked = false;

        Invoke("CheckValue", 0.2f);
    }

    private void OnEnable()
    {
        CheckValue();
    }

    public void CheckValue()
    {
        if (!haveChecked)
        {
            haveChecked = true;
            Invoke("ResetCheck", 0.2f);

            foreach (UiButtonCostRefresh button in allButtons)
            {
                button.CheckValue();
            }

            int cost = ReturnCost();

            if (ShopManager.instance.currentGold >= cost)
            {
                costText.color = greenColor;
            }

            else
            {
                costText.color = redColor;
            }

            SetText(cost);
        }
    }  

    private void ResetCheck()
    {
        haveChecked = false;
    }

    private void SetText(int cost)
    {
        switch (type)
        {
            case ButtonType.jumpSpeed:
                {
                    /*
                    int index = UiFunctions.instance.jumpSpeedIndex;

                    if (index >= 4)
                    {
                        costText.text = "MAX";
                        transform.GetComponent<Button>().interactable = false;
                    }

                    else
                     costText.text = cost.ToString();
                    */

                    costText.text = cost.ToString();

                    float valuePerSec = 1 / CharacterBehaviour.instance.jumpSpeed;

                    valueText.text = String.Format("{0:0.0}", valuePerSec)  + "/s";

                    return;
                }

            case ButtonType.passiveIncome:
                {
                    /*
                    int index = PassiveIncome.instance.passiveIncomeIndex;

                    if (index >= 4)
                    {
                        costText.text = "MAX";
                        transform.GetComponent<Button>().interactable = false;
                    }

                    else
                        costText.text = cost.ToString();
                    */

                    costText.text = cost.ToString();

                    /*
                    int localindex = index;

                    if(localindex >= 4)
                    {
                        localindex = 3;
                    }
                    */

                    int valuePerHour =  PassiveIncome.instance.actualGoldPerHour;

                    valueText.text = valuePerHour.ToString() + "/h";

                    return;
                }

            case ButtonType.increaseSize:
                {
                    int index = UiFunctions.instance.printerScaleIndex;

                    if (index >= 3)
                    {
                        costText.text = "MAX";
                        transform.GetComponent<Button>().interactable = false;
                    }

                    else
                        costText.text = cost.ToString();

                    return ;
                }

            case ButtonType.buyMoney:
                {
                    return ;
                }
        }
    }

    private int ReturnCost()
    {
        switch (type)
        {
            case ButtonType.jumpSpeed:
                {
                    return ShopCostHelper.instance.actualJumpSpeedCost;
                }

            case ButtonType.passiveIncome:        
                {
                    return ShopCostHelper.instance.actualIncomeCost;
                }

            case ButtonType.increaseSize:
                {
                    int index = UiFunctions.instance.printerScaleIndex;

                    if (index < 3)
                        return ShopCostHelper.instance.sizeIncreaseCost[index];
                    else
                        return 0;
                }

            case ButtonType.buyMoney:
                {
                    return ShopCostHelper.instance.moneyShopCost[0];
                }
        }

        return 0;
    }
}

public enum ButtonType
{
    jumpSpeed , 
    passiveIncome,
    increaseSize,
    buyMoney
}
