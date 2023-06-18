using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameMultiplyStation : MonoBehaviour
{
    public int index;
    public int unlockLevel;

    public int unlockCost;
    public int upgradeCostDelta;

    [Header("Local References")]
    public GameObject activeObject;
    public GameObject inactiveObject;
    public GameObject unlockButton;
    public GameObject lockedButon;
    public ParticleSystem levelUpParticle;
    public GameObject model;
    public GameObject updateButton;

    [Header("Local UI")]
    public TMP_Text levelText;
    public TMP_Text outcomeText;
    public TMP_Text levelUpCostText;

    [HideInInspector]
    public int objectLevel;

    private string saveKey;
    private float localCost;

    private void Awake()
    {
        activeObject.SetActive(false);
        inactiveObject.SetActive(true);

        saveKey = "Invest" + index.ToString();
    }


    private void Start()
    {
        if (PlayerPrefs.HasKey(saveKey))
            objectLevel = PlayerPrefs.GetInt(saveKey);

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

    public void UnlockMachine()
    {
        ShopManager.instance.SpendCoin(unlockCost);

        objectLevel = 1;

        SetupCost();
        SetupMachine();

        EventManager.TriggerEvent(Events.saveInvest);
    }

    public bool CanUnlock()
    {
        if (ShopManager.instance.currentGold >= unlockCost)
            return true;

        else
            return false;
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

        levelText.text = "Level " + objectLevel.ToString();
        outcomeText.text = ((objectLevel) * 5).ToString();

        EventManager.TriggerEvent(Events.saveInvest);
    }

    private void SetupMachine()
    {
        if (objectLevel != 0)
        {
            activeObject.transform.localScale = Vector3.zero;
            activeObject.SetActive(true);
            activeObject.transform.DOScale(Vector3.one, 0.5f);

            inactiveObject.SetActive(false);
            
            levelText.text = "Level " + objectLevel.ToString();
            outcomeText.text = ((objectLevel) * 5).ToString();
        }
        else
        {
            if(unlockLevel <= GameManager.instance.CurrentLevel)
            {
                unlockButton.gameObject.SetActive(true);
                lockedButon.gameObject.SetActive(false);
            }
         
            else 
            {
                lockedButon.gameObject.SetActive(true);
                unlockButton.gameObject.SetActive(false);
            }
        }
    }

    private void SetupCost()
    {
        if (objectLevel != 0)
        {
            localCost = unlockCost;

            for (int i = 0; i < objectLevel; i++)
            {
                localCost += upgradeCostDelta;
            }
        }

        localCost = (int)localCost; 
       
        levelUpCostText.text = localCost.ToString();      
    }

    private void OnSaveInvest(object sender)
    {
        PlayerPrefs.SetInt(saveKey, objectLevel);
    }
}
