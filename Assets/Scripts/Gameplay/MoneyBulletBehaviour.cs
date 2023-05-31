using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoneyBulletBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float disableDelay;

    private DecalProjector decal;
    private int value;

    private void OnEnable()
    {
        StartCoroutine(DisableThisGameobject());
        StartCoroutine(MoveForward());
    }

    public void SetValue(int newValue)
    {
        value = newValue;

        decal = transform.GetComponent<DecalProjector>();
        decal.material = ColorUtilities.instance.GetIndexMaterial(value);
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
