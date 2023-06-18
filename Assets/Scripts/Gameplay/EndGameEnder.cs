using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameEnder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            EventManager.TriggerEvent(Events.endGame);

            Invoke("DelayNewScene", 0.5f);
        }
    }

    private void DelayNewScene()
    {
        GameManager.instance.LoadLevel();

        EventManager.TriggerEvent(Events.saveValue);
    }
}