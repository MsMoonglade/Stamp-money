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

        while(Vector3.Distance(transform.position , CharacterBehaviour.instance.transform.position) <= 10)
        {
            yield return new WaitForSeconds(1);
            yield return null;
        }

        CharacterBehaviour.instance.RemoveDecalPositionInList(transform.position);
        this.gameObject.SetActive(false);
    }

    private IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(0.1f);

        while (this.gameObject.activeInHierarchy)
        {
            transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
