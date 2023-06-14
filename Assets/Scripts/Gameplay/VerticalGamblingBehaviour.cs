using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalGamblingBehaviour : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public GameObject endGameDiamondSpawner;
    public GameObject endGameDiamondPrefs;
    public GameObject endGameDiamondParent;
    public GameObject endGameObstaclesParent;

    private Coroutine endGamecoroutine;
    private int endGameBonusGold;

    private void Awake()
    {
        endGamecoroutine = null;
    }

    public void StartGambling()
    {
        EndGameCharacterBehaviour.instance.canMove = false;
        virtualCamera.Priority = 1000;

        if (endGamecoroutine == null)
            endGamecoroutine = StartCoroutine(GamblingBehaviour());
    }

    public void StopGambling()
    {
        EndGameCharacterBehaviour.instance.canMove = true;
        virtualCamera.Priority = 0;
    }
    public void IncreaseEndGameBonusGold(int amount)
    {
        endGameBonusGold += amount;
    }

    private IEnumerator GamblingBehaviour()
    {
        yield return new WaitForSeconds(0.5f);

        endGameDiamondSpawner.GetComponent<MoveEndGameSpawner>().StartMove();

        int endGameDiamond = EndGameCharacterBehaviour.instance.CurrentDiamond();

        for (int i = 0; i < endGameDiamond; i++)
        {
            EndGameCharacterBehaviour.instance.RemoveDiamond(1 );

            GameObject diamond = Instantiate(endGameDiamondPrefs, endGameDiamondSpawner.transform.localPosition, Quaternion.identity, endGameDiamondParent.transform);
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
                   
        ShopManager.instance.IncreaseGold(endGameBonusGold, EndGameCharacterBehaviour.instance.transform.gameObject);
      
        yield return new WaitForSeconds(0.5f);
        StopGambling();
    }
}