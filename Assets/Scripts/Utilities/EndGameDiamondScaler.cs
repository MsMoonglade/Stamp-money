using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class EndGameDiamondScaler : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = new Vector3(0.1f , 0.1f, 0.1f);

        StartCoroutine(ScaleUp());
    }

    IEnumerator ScaleUp()
    {
        yield return new WaitForSeconds(0.3f);

        Vector3 initialScale = transform.localScale;
        float duration = 0.25f;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            float progress = time / duration;
            transform.localScale = Vector3.Lerp(initialScale, new Vector3(1,1,1), progress);
            yield return null;
        }   

        yield return new WaitForEndOfFrame();

        transform.localScale = new Vector3(1,1,1);
    }
}