using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UiBounceTween : MonoBehaviour
{
    private void Awake()
    {
        transform.DOScale(new Vector3(1.25f , 1.25f , 1.25f) , 2)
            .SetLoops(-1 , LoopType.Yoyo);
    }
}
