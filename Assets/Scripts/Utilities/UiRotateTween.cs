using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiRotateTween : MonoBehaviour
{
    public float rotationTime;

    private void Awake()
    {
        transform.DOLocalRotate(new Vector3(0, 0, 360), rotationTime , RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
