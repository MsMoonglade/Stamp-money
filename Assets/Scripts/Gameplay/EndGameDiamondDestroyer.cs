using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameDiamondDestroyer : MonoBehaviour
{
    public int multiplyer;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        int amountMultiply = 5 * multiplyer;
        EndGameBehaviour.instance.IncreaseEndGameBonusGold(amountMultiply);
    }
}
