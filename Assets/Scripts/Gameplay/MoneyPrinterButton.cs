using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPrinterButton : MonoBehaviour
{
    public MoneyPrinterBehaviour thisMachine;

    private void Awake()
    {
        thisMachine.mybutton = this.gameObject;
    }

    public void Print()
    {
        thisMachine.TakeMoney();
    }
}
