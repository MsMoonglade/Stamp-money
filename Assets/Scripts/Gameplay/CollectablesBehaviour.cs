using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesBehaviour : MonoBehaviour
{
    public bool isMoney;
    public bool isDiamond;

    public int rewardAmount;

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player"))
        {
            Take();
        }
    }
      
    private void Take()
    {
        if (isDiamond)
            ShopManager.instance.IncreaseDiamond((int)rewardAmount);

        if (isMoney)
            ShopManager.instance.IncreaseGold((int)rewardAmount, CharacterBehaviour.instance.gameObject);

        this.gameObject.SetActive(false);
    }
}