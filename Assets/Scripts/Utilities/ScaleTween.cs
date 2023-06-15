using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public Vector3 endsScale;
    public float duration;

    public bool loop;

    private void Awake()
    {
        if (loop)
        {
            transform.DOScale(endsScale, duration)
                .SetLoops(-1, LoopType.Yoyo);
        }

        else
        {
            transform.DOScale(endsScale, duration);
        }
    }
}
