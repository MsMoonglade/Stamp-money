using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InvestMachineBehaviour : MonoBehaviour
{
    public int unlockLevel;
    public int index;
    public GameObject reward;
    public GameObject rewardPos;
    public GameObject rewardParent;

    [Header("Local References")]
    public GameObject activeObject;
    public GameObject inactiveObject;
    public TMP_Text costText;
    public TMP_Text levelText;
    public ParticleSystem particle;
    public GameObject atmObject;

    [Header ("Upgrade Variables")]
    public float nextLevelMultiplyer;
    public float initialCost;
    public float initialReward;

    public int machineLevel = 0;
    private string saveKey;

    private float localCost;

    private void Awake()
    {
        activeObject.SetActive (false);
        inactiveObject.SetActive (true);

        saveKey = "Invest" + index.ToString();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.saveInvest, OnSaveInvest);

        if (PlayerPrefs.HasKey(saveKey))
            machineLevel = PlayerPrefs.GetInt(saveKey);

        SetupCost();

        SetupMachine();
        RewardSetup();
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.saveInvest, OnSaveInvest);
    }

    public void UnlockMachine()
    {
        machineLevel = 1;

        SetupCost();        
        SetupMachine();

        EventManager.TriggerEvent(Events.saveInvest);
    }

    public bool CanUpdate()
    {
        if(EndGameCharacterBehaviour.instance.CurrentDiamond() >= (int)localCost)
            return true;

        else
            return false;
    }

    public void LevelUpMachine()
    {
        EndGameCharacterBehaviour.instance.RemoveDiamond((int)localCost, costText.gameObject);

        machineLevel++;

        particle.Play();

        atmObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 0.2f, RotateMode.FastBeyond360)       
            .SetRelative(true)       
            .SetEase(Ease.Linear);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(atmObject.transform.DOScale(new Vector3(0, 0, 0), 0.2f)
            .SetEase(Ease.InBack));
        
        // mySequence.PrependInterval(1);
      
        mySequence.Append(atmObject.transform.DOScale(new Vector3(1, 1, 1), 0.2f)
           .SetEase(Ease.OutBack));

        SetupCost();

        levelText.text = "Lv " + machineLevel.ToString();
        costText.text = localCost.ToString();

        EventManager.TriggerEvent(Events.saveInvest);
    }

    private void SetupMachine()
    {
        if(machineLevel != 0)
        {
            activeObject.SetActive(true);
            inactiveObject.SetActive(false);

            levelText.text = "Lv " + machineLevel.ToString();
            costText.text = localCost.ToString();
        }
    }

    private void RewardSetup()
    {
        if(machineLevel != 0)
        {
            int rewardAmount = (int)(initialReward + machineLevel);
           
            for(int i = 0; i < rewardAmount; i++)
            {
                Vector3 randomizedPos = rewardPos.transform.position + new Vector3(Random.Range(-2f, 2f), 1, Random.Range(-2f, 2f));

                Instantiate(reward, randomizedPos, Quaternion.identity, rewardParent.transform);
            }
        }
    }

    private void SetupCost()
    {
        if (machineLevel != 0)
        {
            localCost = initialCost;

            for (int i = 0; i < machineLevel; i++)
            {
                initialCost *= nextLevelMultiplyer;
            }

            localCost += initialCost;
        }

        localCost = (int)localCost;
    }

    private void OnSaveInvest(object sender)
    {
        PlayerPrefs.SetInt(saveKey, machineLevel);
    }
}