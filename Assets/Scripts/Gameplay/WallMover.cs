using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMover : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMoveX(Mathf.Abs(transform.localPosition.x), moveSpeed)   
            .SetEase(Ease.InOutSine)    
            .SetLoops(-1, LoopType.Yoyo);
    }

}
