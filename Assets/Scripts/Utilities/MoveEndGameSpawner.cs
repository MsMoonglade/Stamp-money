using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEndGameSpawner : MonoBehaviour
{
    public float moveSpeed;

    public void StartMove()
    {
        transform.DOLocalMoveX(Mathf.Abs(transform.localPosition.x), moveSpeed)
          .SetEase(Ease.InOutSine)
          .SetLoops(-1, LoopType.Yoyo);
    }
}
