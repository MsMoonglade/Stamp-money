using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MoneyBulletBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public ParticleSystem particle;

    private DecalProjector decal;

    [HideInInspector]
    public int value;

    private void OnEnable()
    {
        if (GameManager.instance.IsInGameStatus())
        {
            StartCoroutine(DisableThisGameobject());
            StartCoroutine(MoveForward());
        }
    }

    public void SetValue(int newValue)
    {
        value = newValue;

        decal = transform.GetComponent<DecalProjector>();
        decal.material = ColorUtilities.instance.GetIndexMaterial(value);

        Color c = particle.GetComponent<ParticleSystemRenderer>().material.GetColor("_BaseColor");
        c = ColorUtilities.instance.GetIndexColor(value); ;
        particle.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", c);
    }

    private IEnumerator DisableThisGameobject()
    {
        yield return new WaitForSeconds(CharacterBehaviour.instance.bulletActiveTime);       

        this.gameObject.SetActive(false);
    }

    private IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(0.035f);

        while (this.gameObject.activeInHierarchy)
        {
            transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
