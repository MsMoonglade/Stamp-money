using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDecal : MonoBehaviour
{
    public float disableDelay;

    private void OnEnable()
    {
        StartCoroutine(DisableThisGameobject());
    }

    private IEnumerator DisableThisGameobject()
    {
        yield return new WaitForSeconds(disableDelay);

        this.gameObject.SetActive(false);
    }
}
