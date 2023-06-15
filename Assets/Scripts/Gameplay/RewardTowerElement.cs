using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lofelt.NiceVibrations;

public class RewardTowerElement : MonoBehaviour
{
    [Header("EDIT")]
    public int value;

    public bool rewardIsEnergy;
    public bool rewardIsDiamond;
    public bool rewardIsCoin;

    public float rewardAmount;

    [Header("NOT EDIT")]
    public bool fixedValue;
    public float rewardModelOffset;

    public float elementRotOffset;
    public float elementYOffset;
    public float rewardYOffset;

    public GameObject towerParent;
    public GameObject rewardParent;
    public GameObject towerElementPrefs;

    public TMP_Text valueText;

    public ParticleSystem completeParticle;

    private float movedOffset;
    private int startValue;
    private Collider col;

    private void Start()
    {
        movedOffset = 0;
        startValue = value;

        col = GetComponent<Collider>();

        GenerateElement();
        transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);

        //ANIMATE Reward
        rewardParent.transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        valueText.text = value.ToString() + "$";
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Bullet"))
        {
            int val = col.transform.GetComponent<MoneyBulletBehaviour>().value;

            value -= val;
            valueText.text = value.ToString() + "$";

            TweenTowerScale();
            CheckHp();

            col.transform.GetComponent<MoneyBulletBehaviour>().DisableByCollision();
        }

        if (col.transform.CompareTag("Player"))
        {
            CharacterBehaviour.instance.Die();
        }
    }

    private void CheckHp()
    {
        float actualY = (movedOffset * value)/ startValue;
        actualY -= movedOffset;

        towerParent.transform.DOLocalMoveY(actualY, 0.1f);   
        rewardParent.transform.DOLocalMoveY(actualY, 0.1f);

        if (value <= 0)
        {
            completeParticle.Play();
            value = 0;
            col.enabled = false;

            Complete();

            valueText.text = value.ToString() + "$";

            rewardParent.transform.DOScale(0, 0.6f)
                .SetEase(Ease.InBounce)
                .OnComplete(() => this.gameObject.SetActive(false));
        }
    }

    private void Complete()
    {
        if(rewardIsDiamond)        
            ShopManager.instance.IncreaseDiamond((int)rewardAmount , transform.gameObject);

        if(rewardIsCoin)
            ShopManager.instance.IncreaseGold((int)rewardAmount , transform.gameObject);

        if (rewardIsEnergy)
            ShopManager.instance.IncreaseEnergy(rewardAmount , transform.gameObject);

    }

    private void GenerateElement()
    {
        /*
        int valuePerElementMultiplyer = 1;

        int count = value / 200;

        int originalValue = valuePerElement;


        if (count > 0)
        {
            for(int i = 0; i < count; i++)
            {
                valuePerElementMultiplyer++;
                valuePerElement = 0;
            }
        }

        for(int i = 0; i < valuePerElementMultiplyer; i++)
        {
            valuePerElement += originalValue;
        }
        */

        int valuePerElement = 1;

        if (value <= 30)
            valuePerElement = 2;
        else if (value > 30 && value <= 80)
            valuePerElement = 5;

        else if (value > 80 && value <= 150)
            valuePerElement = 6;

        else
            valuePerElement = 10;

        if (fixedValue)
            valuePerElement = 5;

        int necesaryElement = (int)(value / valuePerElement);
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < necesaryElement; i++)
        {
            GameObject element = Instantiate(towerElementPrefs, pos, Quaternion.identity, towerParent.transform);
            element.transform.localPosition = pos;
            element.transform.localRotation *= Quaternion.Euler(0, (elementRotOffset * i), 0);

            pos += new Vector3(0, elementYOffset, 0);
            movedOffset += elementYOffset;
        }

        rewardParent.transform.localRotation *= Quaternion.Euler(0, (elementRotOffset * necesaryElement ), 0);

        pos += new Vector3(0, rewardYOffset, 0);
        pos += new Vector3(0, rewardModelOffset, 0);
        movedOffset += rewardYOffset;

        rewardParent.transform.GetChild(0).transform.localPosition = pos + new Vector3(0, 0.5f, 0);
        rewardParent.transform.GetChild(1).transform.localPosition = pos + new Vector3(0,-1, 0);
    }
    private void TweenTowerScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScale(1.1f, 0.03f));
        mySequence.Append(transform.DOScale(1, 0.03f));

        /*
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(towerParent.transform.DOScale(1.1f, 0.03f));
        mySequence.Append(towerParent.transform.DOScale(1, 0.03f));
        */

        // rewardParent.transform.DOLocalMoveY(0.1f, 0.15f).SetLoops(2 , LoopType.Yoyo);
        // rewardParent.transform.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);
    }
}