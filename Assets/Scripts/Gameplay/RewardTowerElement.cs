using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTowerElement : MonoBehaviour
{
    [Header("EDIT")]
    public int value;

    public bool rewardIsEnergy;
    public bool rewardIsDiamond;
    public bool rewardIsCoin;

    public float rewardAmount;

    [Header("NOT EDIT")]
    public float rewardModelOffset;
    public int valuePerElement;

    public float elementRotOffset;
    public float elementYOffset;
    public float rewardYOffset;

    public GameObject towerParent;
    public GameObject rewardParent;
    public GameObject towerElementPrefs;

    public ParticleSystem completeParticle;

    private float movedOffset;
    private int startValue;
    private Collider col;

    private void Awake()
    {
        movedOffset = 0;
        startValue = value;

        col = GetComponent<Collider>();

        GenerateElement();
        transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);


        //ANIMATE Reward
        rewardParent.transform.DOLocalRotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Bullet"))
        {
            int val = col.transform.GetComponent<MoneyBulletBehaviour>().value;

            value -= val;

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

            rewardParent.transform.DOScale(0, 0.6f)
                .SetEase(Ease.InBounce)                
                .OnComplete(Complete);
        }
    }

    private void Complete()
    {
        ShopManager.instance.IncreaseDiamond((int)rewardAmount);

        this.gameObject.SetActive(false);
    }

    private void GenerateElement()
    {
        int necesaryElement = value / valuePerElement;
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < necesaryElement; i++)
        {
            GameObject element = Instantiate(towerElementPrefs, pos, Quaternion.identity, towerParent.transform);
            element.transform.localPosition = pos;
            element.transform.localRotation *= Quaternion.Euler(0, (elementRotOffset * i), 0);

            pos += new Vector3(0, elementYOffset, 0);
            movedOffset += elementYOffset;
        }

        pos += new Vector3(0, rewardYOffset, 0);
        pos += new Vector3(0, rewardModelOffset, 0);
        movedOffset += rewardYOffset;


        rewardParent.transform.GetChild(0).transform.localPosition = pos;
    }
    private void TweenTowerScale()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(towerParent.transform.DOScale(1.055f, 0.03f));
        mySequence.Append(towerParent.transform.DOScale(1, 0.03f));


       // rewardParent.transform.DOLocalMoveY(0.1f, 0.15f).SetLoops(2 , LoopType.Yoyo);
        rewardParent.transform.DOScale(1.1f, 0.15f).SetLoops(2, LoopType.Yoyo);
    }
}