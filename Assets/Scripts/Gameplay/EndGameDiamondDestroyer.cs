using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameDiamondDestroyer : MonoBehaviour
{
    public int multiplyer;
    public GameObject particle;
    public GameObject textToScale;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(particle, transform.position, Quaternion.identity, transform);

        textToScale.transform.DOScale(1.3f, 0.07f)
          .SetEase(Ease.Linear)
          .SetLoops(2, LoopType.Yoyo);

        Destroy(other.gameObject);
        int amountMultiply = 5 * multiplyer;
        EndGameBehaviour.instance.IncreaseEndGameBonusGold(amountMultiply);
    }
}
