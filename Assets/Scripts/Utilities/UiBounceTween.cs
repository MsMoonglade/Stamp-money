using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiBounceTween : MonoBehaviour
{
    public Vector3 scale;
    public float duration;

    private void Awake()
    {
        transform.DOScale(scale , duration)
            .SetLoops(-1 , LoopType.Yoyo);
    }
}
