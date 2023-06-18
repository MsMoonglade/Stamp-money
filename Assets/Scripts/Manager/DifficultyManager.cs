using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{  
    public static DifficultyManager instance;

    public AnimationCurve obstaclesdifficultyPerLevel;
    public AnimationCurve diamondPerLevel;
    public AnimationCurve coinPerLevel;


    public int diamondPerLevelrandomizerRange;
    public int coinPerLevelrandomizerRange;
    public float difficultyPerLevelrandomizerRange;

    public int currentDiamond;
    public int currentGold;
    public float currentDifficulty;

    private void Start()
    {
        instance = this;

        currentDiamond = EvaluateLevelDiamond(GameManager.instance.CurrentLevel);
        currentDiamond += Random.Range(-diamondPerLevelrandomizerRange, diamondPerLevelrandomizerRange);

        currentGold = EvaluateLevelGold(GameManager.instance.CurrentLevel);
        currentGold += Random.Range(-coinPerLevelrandomizerRange, coinPerLevelrandomizerRange);

        currentDifficulty = EvaluateLevelDifficulty(GameManager.instance.CurrentLevel);
        currentDifficulty += Random.Range(-difficultyPerLevelrandomizerRange, difficultyPerLevelrandomizerRange);

        if(UiFunctions.instance.printerScaleIndex == 2)
        {
            currentDifficulty *= 1.3f;
        }
        if (UiFunctions.instance.printerScaleIndex == 3)
        {
            currentDifficulty *= 1.8f;
        }
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

    public int EvaluateLevelGold(int currentLevel)
    {
        int fixedLevel = 0;

        if (currentLevel < 25)
        {
            fixedLevel = currentLevel;
        }

        else
            fixedLevel = 25;

        int value = (int)coinPerLevel.Evaluate(fixedLevel);
        return value;
    }

    public float EvaluateLevelDifficulty(int currentLevel)
    {
        int fixedLevel = 10;

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