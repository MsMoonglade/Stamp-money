using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndGameDestroyerRotator : MonoBehaviour
{
    public float rotationSpeed;

    void Start()
    {
        transform.DOLocalRotate(new Vector3(-360 , 0 , 0), rotationSpeed, RotateMode.FastBeyond360)
                .SetRelative(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
    }
}
