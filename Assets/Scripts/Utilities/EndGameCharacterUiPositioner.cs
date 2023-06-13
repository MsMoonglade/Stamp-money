using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCharacterUiPositioner : MonoBehaviour
{
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - EndGameCharacterBehaviour.instance.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = EndGameCharacterBehaviour.instance.transform.position + offset;
    }
}
