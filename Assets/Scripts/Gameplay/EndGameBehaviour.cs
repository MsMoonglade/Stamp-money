using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EndGameBehaviour : MonoBehaviour
{
    public static EndGameBehaviour instance;

    public CinemachineVirtualCamera coinCamera;
    public CinemachineVirtualCamera playerCamera;
    public GameObject endGameDiamondPrefs;
    public GameObject endGameDiamondBasket;
    public GameObject endGameCameraTarget;

    public GameObject endGameCoinInstantiatePosRef;
    public GameObject endGame_Table_CharacterPosition;

    private Coroutine endGamecoroutine;

    private int endGameBonusGold;

    private bool ended = false;
    private NavMeshSurface navmeshSurface;

    private void Awake()
    {
        instance = this;

        endGamecoroutine = null;
    
        navmeshSurface = GetComponent<NavMeshSurface>();
        navmeshSurface.BuildNavMesh();
    }

    //Called in player
    public void StartEndGame()
    {
        GameManager.instance.SetEndGame();
        UiManager.instance.DisableGameUI();
     
        EndGameCharacterBehaviour.instance.GetComponent<NavMeshAgent>().enabled = true;

        if (endGamecoroutine == null)
            endGamecoroutine = StartCoroutine(EndGameBehaviourCoroutine());
    }

    public void IncreaseEndGameBonusGold(int amount)
    {
        endGameBonusGold += amount;
    }

    public void CallOnEndGameButton()
    {
        if (!ended)
        {
            ended = true;

            ShopManager.instance.IncreaseGold(endGameBonusGold , endGameCoinInstantiatePosRef);

            EventManager.TriggerEvent(Events.endGame);

            Invoke("ClickCallback", 1.5f);           
        }
    }

    private void ClickCallback()
    {
       GameManager.instance.LoadLevel();
    }
    private IEnumerator EndGameBehaviourCoroutine()
    {
        // endGamebonusGoldUi.transform.DOScale(Vector3.one, 0.5f);

        // yield return new WaitForEndOfFrame();

        coinCamera.Priority = 500;

        yield return new WaitForSeconds(0.5f);

        int endGameDiamond = ShopManager.instance.currentDiamond;
        float yPosOffset = 1f;

        for (int i = 0; i < endGameDiamond; i++)
        {
            ShopManager.instance.DecreaseDiamond(1);
            ShopManager.instance.TweenDiamondUi();

            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(UiManager.instance.ui_Diamond_Destination.transform.position);
            Vector3 endPos = new Vector3(0, i * yPosOffset, 0);

            GameObject diamond = Instantiate(endGameDiamondPrefs, endPos, quaternion.identity, endGameDiamondBasket.transform);
            diamond.transform.localPosition = endPos;
            endGameCameraTarget.transform.DOLocalMove(endPos, 0.075f);

            yield return new WaitForSeconds(0.075f);
        }

        yield return new WaitForSeconds(1);

        playerCamera.Priority = 501;

        EndGameCharacterBehaviour.instance.canMove = true;

        CharacterBehaviour.instance.transform.position = endGame_Table_CharacterPosition.transform.position;
        CharacterBehaviour.instance.transform.rotation = endGame_Table_CharacterPosition.transform.rotation;
        CharacterBehaviour.instance.transform.localScale = endGame_Table_CharacterPosition.transform.parent.transform.localScale;
    }
}