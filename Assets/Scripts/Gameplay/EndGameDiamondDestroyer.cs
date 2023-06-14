using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameDiamondDestroyer : MonoBehaviour
{
    public int multiplyer;
    public GameObject particle;
    public GameObject textToScale;

    private VerticalGamblingBehaviour verticalGamblingBehaviour;

    private void Awake()
    {
        verticalGamblingBehaviour = GetComponentInParent<VerticalGamblingBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (multiplyer != 0)
        {
            Instantiate(particle, transform.position, Quaternion.identity, transform);

            textToScale.transform.DOScale(1.3f, 0.07f)
              .SetEase(Ease.Linear)
              .SetLoops(2, LoopType.Yoyo);

            
            int amountMultiply = 1 * multiplyer;
            verticalGamblingBehaviour.IncreaseEndGameBonusGold(amountMultiply);                   
        }

        Destroy(other.gameObject);
    }
}
