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

    private void Awake()
    {
        instance = this;

        if (GENERATE)        
            GenerateLevel();
    }

    private void GenerateLevel()
    {
        float levelLenght = EvaluateLevelLenght(GameManager.instance.CurrentLevel);
        int levelPieces = (int)(levelLenght / mediumElementDistance);

        elementPosition = Vector3.zero;

        for(int i = 0; i < levelLenght; i++)
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