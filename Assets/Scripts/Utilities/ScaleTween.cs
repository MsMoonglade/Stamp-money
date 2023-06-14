using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public Vector3 endsScale;
    public float duration;
    private void Awake()
    {
        transform.DOScale(endsScale, duration)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
