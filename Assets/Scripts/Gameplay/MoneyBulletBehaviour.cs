using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoneyBulletBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float disableDelay;
    public ParticleSystem particle;

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

        Color c = particle.GetComponent<ParticleSystemRenderer>().material.GetColor("_Color");
        c = ColorUtilities.instance.GetIndexColor(value); ;
        GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", c);
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
