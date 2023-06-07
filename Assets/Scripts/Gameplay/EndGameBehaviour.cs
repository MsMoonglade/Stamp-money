using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class EndGameBehaviour : MonoBehaviour
{
    public static EndGameBehaviour instance;

    public CinemachineVirtualCamera virtualCamera;
    public GameObject endGameDiamondPrefs;
    public GameObject endGameDiamondParent;
    public GameObject endGameDiamondSpawner;
    public GameObject endGameUiBackground;
    public GameObject endGameObstaclesParent;

    public GameObject endGameCoinInstantiatePosRef;

    public GameObject endGamebonusGoldUi;
    public TMP_Text endGameBonusGoldText;

    private Coroutine endGamecoroutine;

    private int endGameBonusGold;

    private bool ended = false;

    private void Awake()
    {
        instance = this;

        endGamecoroutine = null;

        endGamebonusGoldUi.transform.localScale = Vector3.zero;
        endGameUiBackground.transform.localScale = Vector3.zero;
    }

    public void StartEndGame()
    {
        GameManager.instance.SetInMenu();
        UiManager.instance.DisableGameUI();

        if (endGamecoroutine == null)
            endGamecoroutine = StartCoroutine(EndGameBehaviourCoroutine());
    }

    public void IncreaseEndGameBonusGold(int amount)
    {
        endGameBonusGold += amount;
        endGameBonusGoldText.text = endGameBonusGold.ToString();
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

        yield return new WaitForSeconds(2.5f);

        endGameObstaclesParent.transform.DOLocalMoveZ(20, 1.5f);

        while (endGameDiamondParent.transform.childCount > 1)
        {
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1);

        StartCoroutine(EndGameUiAnimation());      
    }

    private IEnumerator EndGameUiAnimation()
    {
        yield return null;

        Vector2 anchorMin = new Vector2(0.5f, 0.5f);
        Vector2 anchorMax = new Vector2(0.5f, 0.5f);

        endGameUiBackground.transform.DOScale(Vector3.one, 1);

        endGamebonusGoldUi.GetComponent<RectTransform>().DOAnchorMin(anchorMin, 1);
        endGamebonusGoldUi.GetComponent<RectTransform>().DOAnchorMax(anchorMax, 1);
        endGamebonusGoldUi.transform.DOLocalMove(Vector3.zero, 1);

        endGamebonusGoldUi.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);       
    }
}