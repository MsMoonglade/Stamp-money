using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPrinterButton : MonoBehaviour
{
    public MoneyPrinterBehaviour thisMachine;
    public GameObject button;
    public float animDuration;

    private void Awake()
    {
        thisMachine.mybutton = this.gameObject;
    }

    public void Print()
    {
        thisMachine.TakeMoney();

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(button.transform.DOScaleY(0.8f, animDuration / 2));
        mySequence.Append(button.transform.DOScaleY(1, animDuration / 2));
    }
}