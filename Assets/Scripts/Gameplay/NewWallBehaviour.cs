using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NewWallBehaviour : MonoBehaviour
{
    public bool giveMoneyWall;
    public int moneyToGive;

    public bool fireRateWall;
    public int increaseFireRate;

    public bool fireDistanceWall;
    public int fireDistanceRate;

    public TMP_Text dynamicText;

    public MeshRenderer[] sideElementMeshRenderer;
    public Material positive_Side_Mat;
    public Material negative_Side_Mat;

    public MeshRenderer centerWallMeshRenderer;
    public Material positive_Center_Mat;
    public Material negative_Center_Mat;

    private bool isNegative;

    private void Awake()
    {
        UpdateUi();

        if (giveMoneyWall)
        {
            if(moneyToGive < 0)
            {
                isNegative = true;
            }
        }

        if (fireRateWall)
        {
            if (increaseFireRate < 0)
            {
                isNegative = true;
            }
        }

        if (fireDistanceWall)
        {
            if (fireDistanceRate < 0)
            {
                isNegative = true;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Bullet"))
        {
            int val = col.transform.GetComponent<MoneyBulletBehaviour>().value;

            if (giveMoneyWall)
                moneyToGive += val;

            if (fireRateWall)
                increaseFireRate += val;

            if (fireDistanceWall)
                fireDistanceRate += val;


            col.transform.gameObject.SetActive(false);

            CheckForPositive();
            UpdateUi();
        }

        if (col.transform.CompareTag("Player"))
        {
            if (giveMoneyWall)
            {
                Debug.Log("sooooldi");
            }

            if (fireRateWall)
            {
                float convertedValue = ConvertFireRateValue();
                CharacterBehaviour.instance.jumpSpeed -= convertedValue;
            }

            if (fireDistanceWall)
            {
                float convertedValue = ConvertFireDistanceValue();
                CharacterBehaviour.instance.bulletActiveTime -= convertedValue;
            }
        }
    }

    private void CheckForPositive()
    {
        if (isNegative)
        {
            if (giveMoneyWall)
            {
                if (moneyToGive > 0)
                {
                    SetToPositive();
                }
            }

            if (fireRateWall)
            {
                if (increaseFireRate > 0)
                {
                    SetToPositive();
                }
            }

            if (fireDistanceWall)
            {
                if (fireDistanceRate > 0)
                {
                    SetToPositive();
                }
            }
        }
    }

    private void SetToPositive()
    {
        isNegative = false;

        transform.DOLocalRotate(new Vector3(0, 360 , 0), 0.2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);      
           
        foreach (MeshRenderer rend in sideElementMeshRenderer)
        {
            rend.material = positive_Side_Mat;
        }

        centerWallMeshRenderer.material = positive_Center_Mat;
    }

    private void UpdateUi()
    {
        if (giveMoneyWall)
        {
            TweenTextScale();

            if(moneyToGive > -1)            
                dynamicText.text ="+" + moneyToGive.ToString();
            else
                dynamicText.text =  moneyToGive.ToString();

        }

        if (fireRateWall)
        {
            TweenTextScale();

            if(increaseFireRate > -1)           
                dynamicText.text = "+" + increaseFireRate.ToString();
            else
                dynamicText.text =  increaseFireRate.ToString();

        }

        if (fireDistanceWall)
        {
            TweenTextScale();

            if(fireDistanceRate > -1)           
                dynamicText.text ="+" + fireDistanceRate.ToString();
            else
                dynamicText.text =  fireDistanceRate.ToString();
        }
    }

    private float ConvertFireRateValue()
    {
        float value = 0;

        return value;
    }

    private float ConvertFireDistanceValue()
    {
        float value = 0;

        return value;
    }

    private void TweenTextScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(dynamicText.transform.DOScale(1.3f, 0.16f));
        mySequence.Append(dynamicText.transform.DOScale(1, 0.16f));
    }
}