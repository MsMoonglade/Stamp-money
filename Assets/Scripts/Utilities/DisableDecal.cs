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

        while(Vector3.Distance(transform.position , CharacterBehaviour.instance.transform.position) <= 10)
        {
            yield return new WaitForSeconds(1);
            yield return null;
        }

        CharacterBehaviour.instance.RemoveDecalPositionInList(transform.position);
        this.gameObject.SetActive(false);
    }
}
