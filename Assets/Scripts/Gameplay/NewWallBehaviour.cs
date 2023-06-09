using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Lofelt.NiceVibrations;

public class NewWallBehaviour : MonoBehaviour
{
    public bool giveMoneyWall;
    public int moneyToGive;

    public bool fireRateWall;
    public int increaseFireRate;
    public float multiplyFireRateRate;

    public bool fireDistanceWall;
    public int fireDistanceRate;
    public float multiplyDistanceRate;

    public TMP_Text dynamicText;

    public MeshRenderer[] sideElementMeshRenderer;
    public Material positive_Side_Mat;
    public Material negative_Side_Mat;

    public MeshRenderer centerWallMeshRenderer;
    public Material positive_Center_Mat;
    public Material negative_Center_Mat;

    public float nearWallCheck_Range;
    public LayerMask nearWallCheck_Mask;

    private bool isNegative;

    private List<Collider> nearWall = new List<Collider>();
    private List <Collider> collidedMoney = new List<Collider>();

    private bool colDelay;

    private void Awake()
    {
        UpdateUi();

        CheckIfNegative();
        CheckNearWall();

        colDelay = true;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Bullet") && !collidedMoney.Contains(col))
        {
            collidedMoney.Add(col);
            StartCoroutine(RemoveThisColFromColList(col));

            int val = col.transform.GetComponent<MoneyBulletBehaviour>().value;

            if (giveMoneyWall)
                moneyToGive += val;

            if (fireRateWall && colDelay)
            {
                increaseFireRate++;
                colDelay = false;
                StartCoroutine(ResetColDelay());
            }

            if (fireDistanceWall && colDelay)
            {
                fireDistanceRate++;
                colDelay = false;
                StartCoroutine(ResetColDelay());
            }

            col.transform.GetComponent<MoneyBulletBehaviour>().PlayColParticle();
            // col.transform.GetComponent<MoneyBulletBehaviour>().DisableByCollision();

            CheckForPositive();
            UpdateUi();
        }

        if (col.transform.CompareTag("Player"))
        {
            foreach (var c in nearWall)
            {
                c.enabled = false;

                c.transform.gameObject.GetComponent<NewWallBehaviour>().DisableAnimation();
            }

            if (giveMoneyWall)
            {
                ShopManager.instance.IncreaseGold(moneyToGive , CharacterBehaviour.instance.gameObject);
            }

            if (fireRateWall)
            {
                float convertedValue = ConvertFireRateValue();
                CharacterBehaviour.instance.EditFireRateInLocalGame(convertedValue);
            }

            if (fireDistanceWall)
            {
                float convertedValue = ConvertFireDistanceValue();
                CharacterBehaviour.instance.bulletActiveTime += convertedValue;

                if(convertedValue > 0)               
                    CharacterBehaviour.instance.powerUpParticle.Play();
                else
                    CharacterBehaviour.instance.powerDownParticle.Play();
            }
        }
    }

    private IEnumerator RemoveThisColFromColList(Collider colToRemvov)
    {
        yield return new WaitForSeconds(0.15f);

        collidedMoney.Remove(colToRemvov);
    }

    public void DisableAnimation()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear);

        transform.DOScale(new Vector3(0, 0, 0), 0.2f)
            .SetEase(Ease.InBack);
    }

    private IEnumerator ResetColDelay()
    {
        yield return new WaitForSeconds(0.2f);
        colDelay = true;
    }

    private void CheckNearWall()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, nearWallCheck_Range , nearWallCheck_Mask);
       
        foreach (var hitCollider in hitColliders)
        {
            nearWall.Add(hitCollider);
        }
    }

    private void CheckIfNegative()
    {
        if (giveMoneyWall)
        {
            if (moneyToGive < 0)
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

        value = increaseFireRate * multiplyFireRateRate;

        return value;
    }

    private float ConvertFireDistanceValue()
    {
        float value = 0;

        value = fireDistanceRate * multiplyDistanceRate;

        return value;
    }

    private void TweenTextScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(dynamicText.transform.DOScale(1.3f, 0.16f));
        mySequence.Append(dynamicText.transform.DOScale(1, 0.16f));
    }
}