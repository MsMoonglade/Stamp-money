using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public bool GENERATE;

    public float startGameDistanceOffset;
    public float endGameDistanceOffset;
    public float mediumElementDistance;
    public float minimunDistanceModifyer;
    public float elementiDistanceModifyer;

    public AnimationCurve levelLenght;

    public GameObject wallParent;
    public GameObject rewardTowerParent;
    public GameObject collectableParent;

    public GameObject endGameObject;
    public GameObject[] possibleWall;
    public GameObject[] possibleRewardTower;
    public GameObject[] possibleCollectable;
    public GameObject[] possibleMixed;

    private Vector3 elementPosition;

    private DifficultyManager difficultyManager;

    private void Awake()
    {
        instance = this;


    }

    private void Start()
    {
        difficultyManager = GetComponent<DifficultyManager>();

        if (GENERATE)
        {
            GenerateLevel();

            SetDiamondPerLevelValue();
            SetCoinPerLevelValue();
            SetLevelDifficulty();
        }
    }

    private void GenerateLevel()
    {
        float levelLenght = EvaluateLevelLenght(GameManager.instance.CurrentLevel);
        int levelPieces = (int)(levelLenght / mediumElementDistance);

        elementPosition = Vector3.zero;
        elementPosition += new Vector3(0, 0, startGameDistanceOffset);

        for (int i = 0; i < levelPieces; i++)
        {
            float randomizer = Random.Range(0.0f, 1.0f);
            float elementOffset = 0; 

            if(randomizer >= 0 && randomizer <= 0.25f)
            {
                GameObject element = GenerateRandomElement(possibleWall ,wallParent);
                element.transform.position = elementPosition;
                elementOffset = element.GetComponent<MapGeneratorElementUtilities>().ElementDistanceLenght();
            }

            else if (randomizer >= 0.25f && randomizer <= 0.5f)
            {
                GameObject element = GenerateRandomElement(possibleRewardTower, rewardTowerParent);
                element.transform.position = elementPosition;
                elementOffset = element.GetComponent<MapGeneratorElementUtilities>().ElementDistanceLenght();

            }

            else if (randomizer >= 0.5f && randomizer <= 0.75f)
            {
                GameObject element = GenerateRandomElement(possibleCollectable, collectableParent);
                element.transform.position = elementPosition;
                elementOffset = element.GetComponent<MapGeneratorElementUtilities>().ElementDistanceLenght();
            }
            //MIXED ELEMENT
            else
            {
                GameObject element = GenerateMixedElement(possibleMixed , wallParent , collectableParent , rewardTowerParent);
                element.transform.position = elementPosition;
                elementOffset = element.GetComponent<MapGeneratorElementUtilities>().ElementDistanceLenght();
            }

            float randomZOffset = Random.Range(minimunDistanceModifyer , elementiDistanceModifyer);
            elementPosition += new Vector3(0, 0, elementOffset + randomZOffset);
            //  elementPosition += new Vector3(0, 0, mediumElementDistance + randomZOffset);
        }

        elementPosition += new Vector3(0, 0, endGameDistanceOffset);
        endGameObject.transform.position = elementPosition;
    }

    private GameObject GenerateRandomElement(GameObject[] possiblePool , GameObject parent)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, parent.transform);
        return o;
    }
    private GameObject GenerateMixedElement(GameObject[] possiblePool, GameObject wallParent , GameObject collectablesParent , GameObject towerParent)
    {
        int index = Random.Range(0, possiblePool.Length);

        GameObject o = Instantiate(possiblePool[index], Vector2.zero, Quaternion.identity, transform);

        return o;
    }

    private void SetDiamondPerLevelValue()
    {
        float diamondValue = difficultyManager.currentDiamond;
        diamondValue *= Random.Range(1 , 1.5f);
        diamondValue = (int)(diamondValue);

        RewardTowerElement[] diamondTower = GetComponentsInChildren<RewardTowerElement>();
        CollectablesBehaviour[] diamondCollectables = GetComponentsInChildren<CollectablesBehaviour>();

        int diamondCount = 0;
        
        for(int i = 0; i < diamondCollectables.Length; i++)
        {
            if (diamondCollectables[i].isDiamond)
                diamondCount++;
        }

        List<RewardTowerElement> diamondtowerList = new List<RewardTowerElement>();
        for(int i = 0; i < diamondTower.Length; i++)
        {
            if (diamondTower[i].rewardIsDiamond)
            {
                diamondtowerList.Add(diamondTower[i]);
            }
        }

        diamondValue -= diamondCount;

        if (diamondtowerList.Count != 0 && diamondValue > 5)
        {
            int localDiamond = (int)(diamondValue / diamondTower.Length);

            localDiamond = Mathf.Abs(localDiamond);

            foreach (RewardTowerElement r in diamondtowerList)
            {
                r.rewardAmount = localDiamond + Random.Range(-localDiamond / 2, localDiamond /2 );

                r.rewardAmount = Mathf.Abs(r.rewardAmount);

                if (r.rewardAmount <= 2)
                    r.rewardAmount = 5;
            }
        }

        if(diamondtowerList.Count != 0 && diamondValue <= 5)
        {
            foreach (RewardTowerElement r in diamondtowerList)
            {
                r.rewardAmount = Random.Range(3, 7);
            }
        }        
    }

    private void SetCoinPerLevelValue()
    {
        float coinValue = difficultyManager.currentGold;
        coinValue *= Random.Range(1, 1.5f);
        coinValue = (int)(coinValue);

        RewardTowerElement[] cointower = GetComponentsInChildren<RewardTowerElement>();
        CollectablesBehaviour[] coinCollectables = GetComponentsInChildren<CollectablesBehaviour>();

        int coinCount = 0;

        for(int i = 0; i < coinCollectables.Length; i++)
        {
            if (coinCollectables[i].isMoney)
                coinCount++;
        }

        List<RewardTowerElement> coinTowerList = new List<RewardTowerElement>();
        for (int i = 0; i < cointower.Length; i++)
        {
            if (cointower[i].rewardIsCoin)
            {
                coinTowerList.Add(cointower[i]);
            }
        }

        coinValue -= coinCount;

        if (coinTowerList.Count != 0 && coinValue > 5)
        {
            int localCoin = (int)(coinValue / coinTowerList.Count);

            localCoin = Mathf.Abs(localCoin);

            foreach (RewardTowerElement r in coinTowerList)
            {
                r.rewardAmount = localCoin + Random.Range(-localCoin / 2, localCoin / 2);

                r.rewardAmount = Mathf.Abs(r.rewardAmount);

                if (r.rewardAmount <= 2)
                    r.rewardAmount = 5;
            }
        }

        if (coinTowerList.Count != 0 && coinValue <= 5)
        {
            foreach (RewardTowerElement r in cointower)
            {
                r.rewardAmount = Random.Range(3, 7);
            }
        }
    }

    private void SetLevelDifficulty()
    {
        float levelDifficulty = (float)difficultyManager.currentDifficulty;

        List<RewardTowerElement> tower = new List<RewardTowerElement>();

        for (int i = 0; i < rewardTowerParent.transform.childCount; i++)
        {
            for (int j = 0; j < rewardTowerParent.transform.GetChild(i).transform.childCount; j++)
            {
                RewardTowerElement t = rewardTowerParent.transform.GetChild(i).transform.GetChild(j).GetComponent<RewardTowerElement>();

                tower.Add(t);
            }
        }
        
        if (tower.Count != 0 )
        {
            foreach (RewardTowerElement t in tower)
            {
                t.value = (int)(levelDifficulty + Random.Range(0, levelDifficulty / 2f));
                
                if (t.value < 2)
                    t.value = 2;                

                float randomizer = Random.Range(0.0f, 1.0f);

                if (randomizer <= 0.075f)
                    t.value *= 5;

                else if (randomizer > 0.075f && randomizer <= 0.135f)
                    t.value = (int)(t.value * 3f);

                else if (randomizer > 0.135f && randomizer <= 0.2f)
                    t.value = (int)(t.value * 1.5f);

                levelDifficulty *= 1.05f;
            }
        }
    }

    public float EvaluateLevelLenght(int currentLevel)
    {
        int fixedLevel = 0;

        if (currentLevel < 25)
        {
            fixedLevel = currentLevel;
        }

        else
            fixedLevel = 25;

        float lenght = levelLenght.Evaluate(fixedLevel);
        return lenght;
    }
}