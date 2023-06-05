using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFunctions : MonoBehaviour
{
    //PRITER SCALE
    private Vector3 printerScale;
    private int printerScaleIndex;

    //JUMP SPEED
    public float[] jumpSpeedAddPerLevel;
    private int jumpSpeedIndex;

    private void Awake()
    {
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
    }

    private void Start()
    {
        printerScale = CharacterBehaviour.instance.printerObject.transform.localScale;  
    }

    public void BuyMoney()
    {
        InventoryManager.Instance.GenerateNewMoney();
    } 

    public void IncreasePrinterSize()
    {
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

    public void IncreaseFireRate()
    {
        if (jumpSpeedIndex < jumpSpeedAddPerLevel.Length)
        {
            float amount = jumpSpeedAddPerLevel[jumpSpeedIndex];
            CharacterBehaviour.instance.IncreaseJumpSpeed(amount);

            jumpSpeedIndex++;
            PlayerPrefs.SetInt("JumpSpeedIndex", jumpSpeedIndex);
        }
    }
}
