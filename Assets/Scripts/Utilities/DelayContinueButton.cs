using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayContinueButton : MonoBehaviour
{
    public GameObject buttonToDelay;
    public GameObject texTToDelay;

    public float delayTime;

    private void OnEnable()
    {
        StartCoroutine(DelayCorutine());
    }

    private IEnumerator DelayCorutine()
    {
        yield return new WaitForSeconds(delayTime);

        buttonToDelay.SetActive(true);
        texTToDelay.SetActive(true);
    }
}