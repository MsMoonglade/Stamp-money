using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Obscurer : MonoBehaviour
{
    public Color endColor;

    private Image obscurer;

    private void Awake()
    {
        obscurer = GetComponent<Image>();

        Clear();
    }
    private void Clear()
    {
        obscurer.DOColor(endColor, 0.2f)
            .OnComplete(() => obscurer.enabled = false);
    }
}