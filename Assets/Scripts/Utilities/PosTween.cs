using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PosTween : MonoBehaviour
{
    public Vector3 endPos;
    public float duration;

    private void Awake()
    {
        transform.DOLocalMoveY(endPos.y, duration)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
