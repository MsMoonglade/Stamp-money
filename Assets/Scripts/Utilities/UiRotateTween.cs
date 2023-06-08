using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiRotateTween : MonoBehaviour
{
    public bool up;

    public float rotationTime;

    private void Awake()
    {
        if (!up)
        {
            transform.DOLocalRotate(new Vector3(0, 0, 360), rotationTime, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        else
        {
            transform.DOLocalRotate(new Vector3(0, 360 , 0), rotationTime, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
