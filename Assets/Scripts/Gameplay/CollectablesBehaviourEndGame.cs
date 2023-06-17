using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesBehaviourEndGame : MonoBehaviour
{
    public float takeDistance;

    private bool taken;

    private void OnEnable()
    {
        taken = false;
    }

    private void Update()
    {
        if (!taken)
        {
            if (Vector3.Distance(transform.position, EndGameCharacterBehaviour.instance.transform.position) <= takeDistance)
            {
                Take();
                taken = true;
            }
        }
    }

    private void Take()
    {    
        ShopManager.instance.IncreaseGold(1, transform.gameObject);

        transform.DOScale(Vector3.zero, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() => this.gameObject.SetActive(false));
    }
}
