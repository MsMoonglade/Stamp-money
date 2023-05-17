using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallMovement : MonoBehaviour
{
    private void Awake()
    {
        transform.DOMoveX(-transform.position.x, 2)
            .SetLoops(-1 , LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
