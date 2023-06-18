using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.Rendering.DebugUI;

public class EndGameMultiplyerActualCol : MonoBehaviour
{
    public GameObject animatingModel;

    private EndGameMultiplyStation parent;
    
    private List<GameObject> collided = new List<GameObject>();

    private bool animating;

    private void Awake()
    {
        parent = transform.parent.GetComponent<EndGameMultiplyStation>();
    }

    private void Start()
    {
         animating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EndDiamond") && !collided.Contains(other.gameObject))
        {
            Vector3 pos;

            if (!animating)
            {                    
                Sequence ZscaleSequence = DOTween.Sequence();
                ZscaleSequence.Append(animatingModel.transform.DOScale(1.25f, 0.1f));
                ZscaleSequence.Append(animatingModel.transform.DOScale(1, 0.1f));                     

                animating = true;
                Invoke("ResetAnimating", 0.1f);
            }

            //GIVE DIAMOND ON PASS
            /*
            for (int i = 0; i < parent.objectLevel + 1; i++)
            {                
                pos = other.transform.position + new Vector3(Random.Range(-1f , 1f), 0 , 0);
                GameObject o = Instantiate(other.transform.gameObject, pos, Quaternion.identity);
                o.transform.parent = other.transform.parent;
                o.transform.localPosition = other.transform.localPosition + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

                float moveTime = InvestmentBehaviour.instance.endPos.transform.position.z - o.transform.position.z;
                float randomizeSpeed = moveTime / InvestmentBehaviour.instance.diamondSpeed; /*+ Random.Range(-0.1f, 1f);

                o.transform.DOLocalMoveZ(InvestmentBehaviour.instance.endPos.transform.localPosition.z, randomizeSpeed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => InvestmentBehaviour.instance.GenerateMoney(o));

                collided.Add(o);              
            }
            */

            //GIVE GOLD ON PASS           
            ShopManager.instance.IncreaseGold(parent.objectLevel * 5, animatingModel.transform.gameObject , true);
            

            collided.Add(other.gameObject);
        }
    }

    private void ResetAnimating()
    {
        animating = false;
    }
}