using DG.Tweening;
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
        transform.localScale = Vector3.one;

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

    public void DisableByCollision()
    {
        GameObject p = PoolManager.instance.GetParticle(GameManager.instance.particleObj , transform.position);     
        p.GetComponent<ParticleSystemRenderer>().material = ColorUtilities.instance.GetIndexParticleMat(value);
        p.SetActive(true);
        
        this.gameObject.SetActive(false);
    }

    private IEnumerator DisableThisGameobject()
    {
        yield return new WaitForSeconds(CharacterBehaviour.instance.bulletActiveTime);

        transform.DOScaleY(0, 0.2f);         
        transform.DOScaleX(0, 0.2f)
            .OnComplete(() => ThisDisable());
    }

    private void ThisDisable()
    {
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
