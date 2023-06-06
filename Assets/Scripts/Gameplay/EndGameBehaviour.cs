using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class EndGameBehaviour : MonoBehaviour
{   
    public static EndGameBehaviour instance;

    public CinemachineVirtualCamera virtualCamera;
    public GameObject endGameDiamondPrefs;
    public GameObject endGameDiamondParent;
    public GameObject endGameDiamondSpawner;

    private Coroutine endGamecoroutine;

    private void Awake()
    {
        instance = this;

        endGamecoroutine = null;
    }

    public void StartEndGame()
    {
        GameManager.instance.SetInMenu();

        if(endGamecoroutine == null)        
            endGamecoroutine = StartCoroutine(EndGameBehaviourCoroutine());        
    }
    private IEnumerator EndGameBehaviourCoroutine()
    {
        yield return new WaitForEndOfFrame();

        virtualCamera.Priority = 500;

        yield return new WaitForSeconds(0.5f);

        endGameDiamondSpawner.GetComponent<MoveEndGameSpawner>().StartMove();

        for(int i = 0; i < ShopManager.instance.currentDiamond; i++)
        {
            GameObject diamond = Instantiate(endGameDiamondPrefs, endGameDiamondSpawner.transform.position, quaternion.identity, endGameDiamondParent.transform);
            diamond.transform.localScale = Vector3.zero;

            diamond.transform.DOScale(Vector3.one, 0.25f);

            yield return new WaitForSeconds(0.35f);
        }

        yield return new WaitForSeconds(1);

        EventManager.TriggerEvent(Events.endGame);
    }
}