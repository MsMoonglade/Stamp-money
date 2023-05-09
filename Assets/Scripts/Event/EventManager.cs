using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SingleParameterEvent : UnityEvent<object> { }
public class DoubleParameterEvent : UnityEvent<object, object> { }
public class TripleParameterEvent : UnityEvent<object, object, object> { }

public class EventManager : MonoBehaviour
{

    private Dictionary<string, SingleParameterEvent> eventSingleDictionary;
    private Dictionary<string, DoubleParameterEvent> eventDoubleDictionary;
    private Dictionary<string, TripleParameterEvent> eventTripleDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventSingleDictionary == null)
        {
            eventSingleDictionary = new Dictionary<string, SingleParameterEvent>();
        }

        if (eventDoubleDictionary == null)
        {
            eventDoubleDictionary = new Dictionary<string, DoubleParameterEvent>();
        }

        if (eventTripleDictionary == null)
        {
            eventTripleDictionary = new Dictionary<string, TripleParameterEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        SingleParameterEvent thisEvent = null;
        if (instance.eventSingleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new SingleParameterEvent();
            thisEvent.AddListener(listener);
            instance.eventSingleDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<object,object> listener)
    {
        DoubleParameterEvent thisEvent = null;
        if (instance.eventDoubleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new DoubleParameterEvent();
            thisEvent.AddListener(listener);
            instance.eventDoubleDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<object, object, object> listener)
    {
        TripleParameterEvent thisEvent = null;
        if (instance.eventTripleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new TripleParameterEvent();
            thisEvent.AddListener(listener);
            instance.eventTripleDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        SingleParameterEvent thisEvent = null;
        if (instance.eventSingleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening(string eventName, UnityAction<object, object> listener)
    {
        if (eventManager == null) return;
        DoubleParameterEvent thisEvent = null;
        if (instance.eventDoubleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void StopListening(string eventName, UnityAction<object, object, object> listener)
    {
        if (eventManager == null) return;
        TripleParameterEvent thisEvent = null;
        if (instance.eventTripleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /*public static void TriggerEvent(string eventName, object sender = null)
    {
        SingleParameterEvent thisEvent = null;
        if (instance.eventSingleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(sender);
        }
    }

    public static void TriggerEvent(string eventName, object sender = null, object parameter = null)
    {
        DoubleParameterEvent thisEvent = null;
        if (instance.eventDoubleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(sender,parameter);
        }
    }*/

    public static void TriggerEvent(string eventName, object sender = null, object parameter1 = null, object parameter2 = null)
    {
        SingleParameterEvent thisEvent = null;
        if (instance.eventSingleDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(sender);
        }

        DoubleParameterEvent thisOtherEvent = null;
        if (instance.eventDoubleDictionary.TryGetValue(eventName, out thisOtherEvent))
        {
            thisOtherEvent.Invoke(sender, parameter1);
        }

        TripleParameterEvent thisAnotherEvent = null;
        if (instance.eventTripleDictionary.TryGetValue(eventName, out thisAnotherEvent))
        {
            thisAnotherEvent.Invoke(sender, parameter1, parameter2);
        }
    }
}