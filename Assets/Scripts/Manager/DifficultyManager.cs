using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{  
    public static DifficultyManager instance;

    public AnimationCurve obstaclesdifficultyPerLevel;
    public AnimationCurve diamondPerLevel;

    public int diamondPerLevelrandomizerRange;
    public float difficultyPerLevelrandomizerRange;

    public int currentDiamond;
    public float currentDifficulty;

    private void Awake()
    {
        instance = this;

        currentDiamond = EvaluateLevelDiamond(GameManager.instance.CurrentLevel);
        currentDiamond += Random.Range(-diamondPerLevelrandomizerRange, diamondPerLevelrandomizerRange);

        currentDifficulty = EvaluateLevelDifficulty(GameManager.instance.CurrentLevel);
        currentDifficulty += Random.Range(-difficultyPerLevelrandomizerRange, difficultyPerLevelrandomizerRange);
    }

    public int EvaluateLevelDiamond(int currentLevel)
    {
        int fixedLevel = 0;

        if (currentLevel < 25)
        {
            fixedLevel = currentLevel;
        }

        else
            fixedLevel = 25;

        int value = (int)diamondPerLevel.Evaluate(fixedLevel);
        return value;
    }

    public float EvaluateLevelDifficulty(int currentLevel)
    {
        int fixedLevel = 0;

        if (currentLevel < 25)
        {
            fixedLevel = currentLevel;
        }

        else
            fixedLevel = 25;

        float value = (int)obstaclesdifficultyPerLevel.Evaluate(fixedLevel);
        return value;
    }
}