using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloorButtonTrigger : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent startTriggerAction;
    public UnityEvent stopTriggerAction;

    private bool triggered;


    private void Awake()
    {
        triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !triggered)
        {
            triggered = true;
            startTriggerAction.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") && triggered)
        {          
            stopTriggerAction.Invoke();
            triggered = false;
        }
    }
}