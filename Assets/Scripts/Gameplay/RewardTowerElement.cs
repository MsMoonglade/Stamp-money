using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lofelt.NiceVibrations;
using static UnityEditor.PlayerSettings;

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

        transform.localPosition += new Vector3(0, 0.45f, 0);

        if (rewardIsEnergy)
        {
            rewardAmount += Random.Range(-0.3f, 1f);
            rewardAmount = Mathf.Clamp(rewardAmount, 0.1f, 1f);
        }
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

        towerParent.transform.DOLocalMoveY(actualY + 0.2f, 0.3f);   
        rewardParent.transform.DOLocalMoveY(actualY + 0.2f, 0.3f);

        if (value <= 0)
        {
            rewardParent.transform.GetChild(1).transform.gameObject.SetActive(false);
            col.enabled = false;

            valueText.transform.parent.DOScale(Vector3.zero, 0.15f);

            completeParticle.Play();
            value = 0;

            Complete();

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

        /*
        pos += new Vector3(0, rewardYOffset , 0);
        pos += new Vector3(0, rewardModelOffset, 0);
        */

        movedOffset += rewardYOffset;
        movedOffset += rewardModelOffset;

        rewardParent.transform.GetChild(0).transform.localPosition = pos + new Vector3(0 , 1.5f , 0);
        rewardParent.transform.GetChild(1).transform.localPosition = pos ;      
    }
    private void TweenTowerScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScaleX(1.1f, 0.05f));
        mySequence.Append(transform.DOScaleX(1, 0.05f));

        Sequence mySequence2 = DOTween.Sequence();
        mySequence2.Append(transform.DOScaleZ(1.1f, 0.05f));
        mySequence2.Append(transform.DOScaleZ(1, 0.05f));
      
        Sequence mySequence3 = DOTween.Sequence();
        mySequence3.Append(valueText.transform.DOScale(1.1f, 0.05f));
        mySequence3.Append(valueText.transform.DOScale(1f, 0.05f));
    }
}