using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InvestmentBehaviour : MonoBehaviour
{
    public static InvestmentBehaviour instance;

    public GameObject endPos;
    public float diamondSpeed;
    public GameObject diamondStartPos;
    public GameObject diamondParent;
    public GameObject coinRewardParent;

    public GameObject rewardCoinPrefs;
    public int coinPerDiamond;

    public GameObject coinExplosionParticles;
    public GameObject coinExplosionParticlesPos;

    private Coroutine takeDiamondCoroutine;

    private void Awake()
    {
        instance = this;

        takeDiamondCoroutine = null;
    }

    public void TakeDiamond(GameObject diamond)
    {
       // diamond.transform.DOLocalMove(diamondStartPos.transform.localPosition , 1.5f);
       // diamond.transform.DOScale(Vector3.one, 0.15f);

        float moveTime = endPos.transform.position.z - diamond.transform.position.z;
        float randomizeSpeed = moveTime / diamondSpeed; /*+ Random.Range(-0.1f, 1f)*/;

        diamond.transform.DOLocalMoveZ(endPos.transform.localPosition.z , randomizeSpeed)
            .SetEase(Ease.Linear)
            .OnComplete(() => GenerateMoney(diamond));
    }

    public void GenerateMoney(GameObject diamond)
    {
        Instantiate(coinExplosionParticles, coinExplosionParticlesPos.transform.position, Quaternion.identity, coinExplosionParticlesPos.transform);

        diamond.gameObject.SetActive(false);

        for (int i = 0; i < coinPerDiamond; i++) 
        {
            Vector3 randomizedPos = coinRewardParent.transform.position + new Vector3(Random.Range(-8f, 8f), 2f, Random.Range(-4f, 4f));
            Instantiate(rewardCoinPrefs, randomizedPos, Quaternion.identity, coinRewardParent.transform);
        } 
    }

    public void StartTakeDiamond()
    {
        if(takeDiamondCoroutine == null)
        {
            takeDiamondCoroutine = StartCoroutine(TakeDiamondCoroutine());
        }
    }

    public void StopTakeDiamond()
    {
        if(takeDiamondCoroutine != null)
        {
            StopCoroutine(takeDiamondCoroutine);
            takeDiamondCoroutine = null;
        }
    }

    private IEnumerator TakeDiamondCoroutine()
    {
        while (true)
        {
            GameObject d = EndGameCharacterBehaviour.instance.TakeOneDiamond();

            if(d != null)
            {
                d.transform.parent = diamondParent.transform;

                Vector3 randomizedPos = diamondStartPos.transform.localPosition;
                randomizedPos += new Vector3(Random.Range(-1f, 1f) , 0 , 0 );

                d.transform.DOLocalMove(randomizedPos , 1.5f)
                    .OnComplete(() => TakeDiamond(d));

                //  d.transform.DOScale(Vector3.zero, 0.15f)
                //       .OnComplete(() => TakeDiamond(d));
            }

            yield return new WaitForSeconds(0.15f);
        }
    }
}