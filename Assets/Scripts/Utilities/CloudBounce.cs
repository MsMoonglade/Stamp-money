using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudBounce : MonoBehaviour
{
    private void Start()
    {
        transform.DOShakePosition(Random.Range(15,25), strength: new Vector3(0, Random.Range(1, 2), 0), vibrato: 1, randomness: 2, snapping: false, fadeOut: false)
            .SetLoops(-1);
    }
}
