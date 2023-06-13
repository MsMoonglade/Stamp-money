using DG.Tweening;
using MoreMountains.Tools;
using SupersonicWisdomSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject moneyDecalObj;
    public GameObject moneyDecalParent;
    public int moneyDecalCount;

    public GameObject particleObj;
    public GameObject particleParent;
    public int particleCount;

    [HideInInspector]
    public GameStatus gameStatus;

    private int currentLevel;

    [HideInInspector]
    public bool inEdit;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }

        set
        {
            currentLevel = value;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        }
    }

    private void Awake()
    {
        instance = this;

        inEdit = false;

        SetInMenu();

        for(int i = 0; i < moneyDecalCount; i++)
        {
            PoolManager.instance.InstantiateInPool(moneyDecalObj, moneyDecalParent);
        }

        for (int i = 0; i < particleCount; i++)
        {
            PoolManager.instance.InstantiateParticleInPool(particleObj, particleParent);
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

        DOTween.SetTweensCapacity(500, 100);           
    }

    private void Start()
    {
        //currentLevel set
        if (PlayerPrefs.HasKey("CurrentLevel"))
            CurrentLevel = PlayerPrefs.GetInt("CurrentLevel");
        else
        {
            CurrentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", CurrentLevel);
        }    
        
        UiManager.instance.levelText.text = "LEVEL " + CurrentLevel.ToString();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Events.endGame , EndGame);
        EventManager.StartListening(Events.die, OnCharacterDie);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.endGame, EndGame);
        EventManager.StopListening(Events.die, OnCharacterDie);
    }

    public bool LevelPassed()
    {    
        return false;
    }

    public void ResetScore()
    {
    }

    public void StartGame()
    {
        SetInGame();

        UiManager.instance.EnableGameUi();
        EventManager.TriggerEvent(Events.playGame);

        SupersonicWisdom.Api.NotifyLevelStarted(CurrentLevel, null);

        //LevelManager.instance.GenerateLevel(CurrentLevel);
    }

    public void EndGame(object sender)
    {
        SupersonicWisdom.Api.NotifyLevelCompleted(CurrentLevel, null);
        
        EventManager.TriggerEvent(Events.saveValue);
        CurrentLevel++;
        SetInMenu();
        
        //UiManager.instance.EnableEndGameUi();

        //EventManager.TriggerEvent(Events.endGame);
    }

    public void OnCharacterDie(object sender)
    {
        SupersonicWisdom.Api.NotifyLevelFailed(CurrentLevel, null);

        gameStatus = GameStatus.InRestart;
        UiManager.instance.EnableRetryUi();
    }

    public void ReloadLevel()
    {        
        SceneManager.LoadScene(1);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void SetInGame()
    {
        gameStatus = GameStatus.InGame;
    }

    public void SetInMenu()
    {
        gameStatus = GameStatus.InMenu;
    }
    public void SetEndGame()
    {
        gameStatus = GameStatus.InEndGame;
    }    

    public bool IsInGameStatus()
    {
        if (gameStatus == GameStatus.InGame)
            return true;

        else
            return false;
    }

    public bool IsInEndGameStatus()
    {
        if (gameStatus == GameStatus.InEndGame)
            return true;

        else
            return false;
    }

    public void OnApplicationQuit()
    {
    }

    public void EnterEditView()
    {
        CinemachineVirtualCameraSwitcher.instance.SwitchToEditCamera();
        inEdit = true;
        CharacterBehaviour.instance.StartEdit();
    }

    public void ExitEditview()
    {
        CinemachineVirtualCameraSwitcher.instance.SwitchToPlayerCamera();
        CharacterBehaviour.instance.ConfirmEdit();
    }

    public void DetectStartGameButton()
    {
        StartGame();
        InputManager.instance.FirstInput();
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        ReloadLevel();
    }
}

public enum GameStatus
{
    InGame,
    InMenu,
    InRestart,
    InEndGame
}