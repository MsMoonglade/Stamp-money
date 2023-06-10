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
    public float elementiDistanceModifyer;

    public AnimationCurve levelLenght;

    public GameObject wallParent;
    public GameObject rewardTowerParent;
    public GameObject collectableParent;

    public GameObject endGameObject;
    public GameObject[] possibleWall;
    public GameObject[] possibleRewardTower;
    public GameObject[] possibleCollectable;

    private Vector3 elementPosition;

    private DifficultyManager difficultyManager;

    private void Awake()
    {
        instance = this;

        difficultyManager = GetComponent<DifficultyManager>();

        if (GENERATE)
        {
            GenerateLevel();
        }
    }

    private void Start()
    {
        SetDiamondPerLevelValue();
        SetLevelDifficulty();
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

            if(randomizer >= 0 && randomizer <= 0.33f)
            {
                GameObject element = GenerateRandomElement(possibleWall ,wallParent);
                element.transform.position = elementPosition;
            }

            else if (randomizer >= 0.34f && randomizer <= 0.66f)
            {
                GameObject element = GenerateRandomElement(possibleRewardTower, rewardTowerParent);
                element.transform.position = elementPosition;
            }

            else if (randomizer >= 0.67f && randomizer <= 1f)
            {
                GameObject element = GenerateRandomElement(possibleCollectable, collectableParent);
                element.transform.position = elementPosition;
            }

            float randomZOffset = Random.Range(-elementiDistanceModifyer , elementiDistanceModifyer); 
            elementPosition += new Vector3(0, 0, mediumElementDistance + randomZOffset);
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

    private void SetDiamondPerLevelValue()
    {
        float diamondValue = difficultyManager.currentDiamond;
        diamondValue *= Random.Range(1 , 1.5f);
        diamondValue = (int)(diamondValue);

        List<RewardTowerElement> diamondTower = new List<RewardTowerElement> ();

        for(int i = 0; i < rewardTowerParent.transform.childCount; i++)
        {
            for(int j = 0; j < rewardTowerParent.transform.GetChild(i).transform.childCount; j++)
            {
                RewardTowerElement t = rewardTowerParent.transform.GetChild(i).transform.GetChild(j).GetComponent<RewardTowerElement>();
                if (t.rewardIsDiamond)
                {
                    diamondTower.Add(t);
                }
            }
        }

        List<CollectablesBehaviour> diamondCollectables = new List<CollectablesBehaviour>();

        for (int i = 0; i < collectableParent.transform.childCount; i++)
        {
            for (int j = 0; j < collectableParent.transform.GetChild(i).transform.childCount; j++)
            {
                CollectablesBehaviour c = collectableParent.transform.GetChild(i).transform.GetChild(j).GetComponent<CollectablesBehaviour>();
                if (c.isDiamond)
                {
                    diamondCollectables.Add(c);
                }
            }
        }

        diamondValue -= (int)(diamondCollectables.Count /2);

        if (diamondTower.Count != 0 && diamondValue > 5)
        {
            int localDiamond = (int)(diamondValue / diamondTower.Count);

            localDiamond = Mathf.Abs(localDiamond);

            foreach (RewardTowerElement r in diamondTower)
            {
                r.rewardAmount = localDiamond + Random.Range(-localDiamond / 2, localDiamond /2 );

                r.rewardAmount = Mathf.Abs(r.rewardAmount);

                if (r.rewardAmount <= 2)
                    r.rewardAmount = 5;
            }
        }

        if(diamondTower.Count != 0 && diamondValue <= 5)
        {
            foreach (RewardTowerElement r in diamondTower)
            {
                r.rewardAmount = Random.Range(3, 7);
            }
        }
    }

    private void SetLevelDifficulty()
    {
        float levelDifficulty = difficultyManager.currentDifficulty;

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
                t.value = (int)(levelDifficulty + Random.Range(-levelDifficulty / 2, levelDifficulty / 1.5f));

                t.value = Mathf.Abs(t.value);

                float randomizer = Random.Range(0.0f, 1.0f);

                if (randomizer <= 0.065f)
                    t.value *= 5;

                else if (randomizer > 0.065f && randomizer <= 0.125f)
                    t.value = (int)(t.value * 3f);

                else if (randomizer > 0.125f && randomizer <= 0.2f)
                    t.value = (int)(t.value * 1.5f);
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