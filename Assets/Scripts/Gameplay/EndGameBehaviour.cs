using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject endGamebonusGoldUi;
    public TMP_Text endGameBonusGoldText;

    private Coroutine endGamecoroutine;

    private int endGameBonusGold;

    private void Awake()
    {
        instance = this;

        endGamecoroutine = null;

        endGamebonusGoldUi.transform.localScale = Vector3.zero;
    }

    public void StartEndGame()
    {
        GameManager.instance.SetInMenu();
        UiManager.instance.DisableGameUI(); 

        if(endGamecoroutine == null)        
            endGamecoroutine = StartCoroutine(EndGameBehaviourCoroutine());        
    }

    public void IncreaseEndGameBonusGold(int amount)
    {
        endGameBonusGold += amount;
        endGameBonusGoldText.text = endGameBonusGold.ToString();
    }

    private IEnumerator EndGameBehaviourCoroutine()
    {
        endGamebonusGoldUi.transform.DOScale(Vector3.one, 0.5f);

        yield return new WaitForEndOfFrame();

        virtualCamera.Priority = 500;

        yield return new WaitForSeconds(0.5f);

        endGameDiamondSpawner.GetComponent<MoveEndGameSpawner>().StartMove();

        int endGameDiamond = ShopManager.instance.currentDiamond;

        for (int i = 0; i < endGameDiamond; i++)
        {
            ShopManager.instance.DecreaseDiamond(1);
            ShopManager.instance.TweenDiamondUi();

            GameObject diamond = Instantiate(endGameDiamondPrefs, endGameDiamondSpawner.transform.localPosition, quaternion.identity, endGameDiamondParent.transform);
            diamond.transform.localPosition = endGameDiamondSpawner.transform.localPosition;           

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1);

        EventManager.TriggerEvent(Events.endGame);
    }
}