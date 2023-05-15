using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GameObject levelParent;

    public AnimationCurve levelLenght;
    public float levelElementDistance;
    public float levelElementDistanceOffset;
    public float startOffset;

    public GameObject[] possibleLevelElement;

    public GameObject endElement;

    private void Awake()
    {
        instance = this;

        GenerateLevel();
    }

    private void GenerateLevel()
    {
        int levelElement = (int)levelLenght.Evaluate(GameManager.instance.CurrentLevel);

        Vector3 elementPos = Vector3.zero;
        elementPos += new Vector3(0, 0, startOffset);


        for (int i = 0; i < levelElement; i++)
        {
            int randomElementIndex = Random.Range(0, possibleLevelElement.Length);
            GameObject currentRandomElement = possibleLevelElement[randomElementIndex];

            Instantiate(currentRandomElement, elementPos, currentRandomElement.transform.rotation, levelParent.transform);

            float offset = Random.Range(levelElementDistance - levelElementDistanceOffset , levelElementDistance + levelElementDistanceOffset);
            elementPos += new Vector3(0, 0, offset);
        }

        Instantiate(endElement, elementPos, endElement.transform.rotation, levelParent.transform);
    }
}