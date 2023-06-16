using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FloorButtonBehaviour : MonoBehaviour
{
    [Header("Events")]
    public EndGameMultiplyStation requisites;
    public bool updateReq;
    public UnityEvent actionToTrigger;

    [Header("PublicVariables")]
    public float necessaryTime;

    [Header("LocalVariables")]
    public Image activeImage;

    private bool triggered;
    private float timer;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        activeImage.fillAmount = 0;
        timer = 0;
        timerCoroutine = null;

        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (requisites == null)
        {
            if (other.transform.CompareTag("Player") && !triggered)
            {
                if (timerCoroutine == null)
                    timerCoroutine = StartCoroutine(StartCounter());
            }
        }

        else
        {
            if (updateReq)
            {
                if (requisites.CanUpdate())
                {
                    if (other.transform.CompareTag("Player") && !triggered)
                    {
                        if (timerCoroutine == null)
                            timerCoroutine = StartCoroutine(StartCounter());
                    }
                }
            }

            else
            {
                if (requisites.CanUnlock())
                {
                    if (other.transform.CompareTag("Player") && !triggered)
                    {
                        if (timerCoroutine == null)
                            timerCoroutine = StartCoroutine(StartCounter());
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (timerCoroutine != null)
            {
                timer = 0;
                
                if (timerCoroutine != null)
                {
                    StopCoroutine(timerCoroutine);
                    timerCoroutine = null;
                }

                activeImage.fillAmount = 0;
            }

            triggered = false;
        }
    }

    private void TriggerAction()
    {

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        timer = 0;
        activeImage.fillAmount = 0;

        triggered = true;
        actionToTrigger?.Invoke();
    }

    private IEnumerator StartCounter()
    {
        while (!triggered)
        {
            if (timer < necessaryTime)
            {
                timer += Time.deltaTime;

                float fillAmount = timer/necessaryTime;

                activeImage.fillAmount = fillAmount;
            }

            if (timer >= necessaryTime && !triggered)
            {
                TriggerAction();
                break;
            }

            yield return null;
        }
    }
}
