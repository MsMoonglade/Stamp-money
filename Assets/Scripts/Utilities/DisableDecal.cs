using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDecal : MonoBehaviour
{
    public float moveSpeed;
    public float disableDelay;

    private void OnEnable()
    {
        StartCoroutine(DisableThisGameobject());
        StartCoroutine(MoveForward());
    }

    private IEnumerator DisableThisGameobject()
    {
        yield return new WaitForSeconds(disableDelay);       

        this.gameObject.SetActive(false);
    }

    private IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(0.075f);

        while (this.gameObject.activeInHierarchy)
        {
            transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
