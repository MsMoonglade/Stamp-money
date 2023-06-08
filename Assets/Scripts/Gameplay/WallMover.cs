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
        float right = Random.Range(0.0f, 1.0f);

        if (right <= 0.5f)
        {
            transform.localPosition = new Vector3(3, transform.localPosition.y, transform.localPosition.z);
           
            transform.DOLocalMoveX(-transform.localPosition.x, moveSpeed)
                   .SetEase(Ease.InOutSine)
                   .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.localPosition = new Vector3(-3, transform.localPosition.y, transform.localPosition.z);

            transform.DOLocalMoveX(Mathf.Abs(transform.localPosition.x), moveSpeed)        
                .SetEase(Ease.InOutSine)        
                .SetLoops(-1, LoopType.Yoyo);
        }

     
     
    }

}
