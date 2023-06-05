using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveIncome : MonoBehaviour
{
    DateTime currentDate;
    DateTime oldDate;

    // Use this for initialization
    void Awake()
    {
        if(PlayerPrefs.GetString("OldHour") != null)    
            CheckOfflinetime();
    }

    public int CheckOfflinetime()
    {
        string savedTime = PlayerPrefs.GetString("OldHour");
        //Debug.Log(savedTime);        
        
        oldDate = System.DateTime.Parse(savedTime);
        currentDate = System.DateTime.Now;

        TimeSpan difference = currentDate.Subtract(oldDate);

        return (int)difference.TotalHours;        
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("OldHour", System.DateTime.Now.ToString());
        print("saving this date to player prefs" + System.DateTime.Now);
    }
}