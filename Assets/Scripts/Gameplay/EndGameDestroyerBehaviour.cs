using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameDestroyerBehaviour : MonoBehaviour
{
    public int unlockLevel;

    public int startCost;
    public int upgradeCostDelta;

    [Header("Local References")]
    public GameObject unlockButton;
    public GameObject lockedButon;
    public ParticleSystem levelUpParticle;
    public GameObject model;

    [Header("Local UI")]
    public TMP_Text outcomeText;
    public TMP_Text levelUpCostText;
    public TMP_Text objectLevelText;

    [HideInInspector]
    public int objectLevel;

    private string saveKey;
    private float localCost;

    private void Awake()
    {
        saveKey = "DiamondDestroyer";
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(saveKey))
            objectLevel = PlayerPrefs.GetInt(saveKey);

        else
        {
            objectLevel = 1;
            PlayerPrefs.SetInt(saveKey, objectLevel);
        }

        InvestmentBehaviour.instance.coinPerDiamond = objectLevel * 10;

        SetupCost();
        SetupMachine();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.saveInvest, OnSaveInvest);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.saveInvest, OnSaveInvest);
    }

    public bool CanUpdate()
    {
        if (ShopManager.instance.currentGold >= (int)localCost)
            return true;
        else
            return false;
    }

    public void LevelUpMachine()
    {
        ShopManager.instance.SpendCoin((int)localCost);

        objectLevel++;
        InvestmentBehaviour.instance.coinPerDiamond = objectLevel * 10;

        levelUpParticle.Play();

        model.transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(model.transform.DOScale(new Vector3(0, 0, 0), 0.2f)
            .SetEase(Ease.InBack));
        mySequence.Append(model.transform.DOScale(new Vector3(1, 1, 1), 0.2f)
           .SetEase(Ease.OutBack));

        SetupCost();

        outcomeText.text = (InvestmentBehaviour.instance.coinPerDiamond).ToString();
        objectLevelText.text = "Level " + objectLevel.ToString();

        EventManager.TriggerEvent(Events.saveInvest);
    }

    private void SetupMachine()
    {
        if (unlockLevel <= GameManager.instance.CurrentLevel)
        {
            unlockButton.gameObject.SetActive(true);
            lockedButon.gameObject.SetActive(false);
        }

        else
        {
            lockedButon.gameObject.SetActive(true);
            unlockButton.gameObject.SetActive(false);
        }

        outcomeText.text = (InvestmentBehaviour.instance.coinPerDiamond).ToString();
        objectLevelText.text = "Level " + objectLevel.ToString();
    }

    private void SetupCost()
    {
        localCost = startCost;

        for (int i = 0; i < objectLevel; i++)
        {
            localCost += upgradeCostDelta;
        }

        localCost = (int)localCost;

        levelUpCostText.text = localCost.ToString();
    }

    private void OnSaveInvest(object sender)
    {
        PlayerPrefs.SetInt(saveKey, objectLevel);
    }
}