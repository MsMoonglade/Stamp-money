using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameMultiplyerActualCol : MonoBehaviour
{
    private List<GameObject> collided = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EndDiamond") && !collided.Contains(other.gameObject))
        {
            Vector3 closePos = other.transform.localPosition + new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
            GameObject o = Instantiate(other.transform.gameObject, closePos, Quaternion.identity, other.transform.parent);
            o.transform.localPosition = closePos;

            float moveTime = InvestmentBehaviour.instance.endPos.transform.localPosition.z - o.transform.position.z;
            float randomizeSpeed = moveTime / InvestmentBehaviour.instance.diamondSpeed; /*+ Random.Range(-0.1f, 1f)*/;

            o.transform.DOLocalMoveZ(InvestmentBehaviour.instance.endPos.transform.localPosition.z, randomizeSpeed)          
                .SetEase(Ease.Linear)           
                .OnComplete(() => InvestmentBehaviour.instance.GenerateMoney(o));

            collided.Add(o);
            collided.Add(other.gameObject);
        }
    }
}