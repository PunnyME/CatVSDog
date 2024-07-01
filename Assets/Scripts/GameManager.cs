using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    SelectMode,
    Playing,
    Processing,
    GameOver
}

public enum PlayerTurn
{
    None,
    Player1Turn,
    Player2Turn
}

public enum Player
{
    Player1,
    Player2
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public enum SpetialItem
{
    PowerThrow,
    DoubleThrow,
    Heal
}

public enum HitResult
{
    Head,
    Body,
    NotHit
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State { get; private set; }
    public PlayerTurn PlayerTurn { get; private set; }

    public bool HasOnePlayer { get; private set; }

    public Difficulty Difficulty { get; private set; }

    public bool IsThrowing { get; private set; }

    public float Wind { get; private set; }

    public ReadGoogleSheet GameData => gameData;

    [SerializeField]
    private PlayerCharacter player1;

    [SerializeField]
    private PlayerCharacter player2;

    [SerializeField]
    private MainGameUIManager mainGameUIManager;

    [SerializeField]
    private GameOverUIManager gameOverUIManager;

    [SerializeField]
    private ReadGoogleSheet gameData;

    private PlayerTurn nextTurn;

    private float timeToThink;
    private float timeToWarning;

    private float initialTimeToThink;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        State = GameState.MainMenu;
        PlayerTurn = PlayerTurn.None;
    }

    public void ChangeGameState(GameState state)
    {
        State = state;
    }

    public void ChangePlayerTurn(PlayerTurn playerTurn)
    {
        PlayerTurn = playerTurn;
    }

    public void SetHasOnePlayer(bool hasOnePlayer)
    {
        this.HasOnePlayer = hasOnePlayer;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        this.Difficulty = difficulty;
    }

    public void StartGame()
    {
        string timeToThinkStr = gameData._GameDataList.Data[9].Sec;
        string timeToWarningStr = gameData._GameDataList.Data[10].Sec;

        timeToThinkStr = timeToThinkStr.Replace("s", "");
        timeToWarningStr = timeToWarningStr.Replace("s", "");

        timeToThink = float.Parse(timeToThinkStr);
        timeToWarning = float.Parse(timeToWarningStr);

        initialTimeToThink = timeToThink;

        StartTurn();
        PlayerTurn = PlayerTurn.Player1Turn;
        nextTurn = PlayerTurn.Player2Turn;
        player1.Init();
        player2.Init();
    }

    public void StartTurn()
    {
        Wind = Random.Range(-5f, 5f);
        mainGameUIManager.UpdateWind(Wind);
        State = GameState.Playing;
        mainGameUIManager.UpdatePlayerTurn();
        timeToThink = initialTimeToThink;

        if (HasOnePlayer && PlayerTurn == PlayerTurn.Player2Turn)
        {
            player2.AIThrow();
        }
        else
        {
            StartCoroutine(TimeCountdown());
        }
    }

    public void TurnEnd()
    {
        StartCoroutine(EndProgress());
    }

    private IEnumerator EndProgress()
    {
        yield return new WaitForSeconds(1.5f);

        if (State != GameState.GameOver)
        {
            player1.ResetTurn();
            player2.ResetTurn();
            (PlayerTurn, nextTurn) = (nextTurn, PlayerTurn);
            StartTurn();
        }
    }

    private IEnumerator TimeCountdown()
    {
        while (timeToThink > 0f)
        {
            if (!IsThrowing)
            {
                yield return new WaitForEndOfFrame();
                timeToThink -= Time.deltaTime;

                if (timeToThink < timeToWarning)
                {
                    if (PlayerTurn == PlayerTurn.Player1Turn)
                    {
                        player1.SetTimeCount(true, timeToThink / timeToWarning);
                    }
                    else if (PlayerTurn == PlayerTurn.Player2Turn)
                    {
                        player2.SetTimeCount(true, timeToThink / timeToWarning);
                    }
                }
            }
            else
            {
                player1.SetTimeCount(false, 0f);
                player2.SetTimeCount(false, 0f);
                break;
            }
        }

        if (timeToThink <= 0f)
        {
            player1.SetTimeCount(false, 0f);
            player2.SetTimeCount(false, 0f);

            if (PlayerTurn == PlayerTurn.Player1Turn)
            {
                player2.SetAnimation("Happy UnFriendly");
            }
            else if (PlayerTurn == PlayerTurn.Player2Turn)
            {
                player1.SetAnimation("Happy UnFriendly");
            }

            TurnEnd();
        }

        yield return null;
    }

    public void MissAnimation(Player player)
    {
        switch (player)
        {
            case Player.Player1:
                player2.SetAnimation("Happy UnFriendly");
                break;
            case Player.Player2:
                player1.SetAnimation("Happy UnFriendly");
                break;
        }
    }

    public void SetIsThrowing(bool isThrowing)
    {
        IsThrowing = isThrowing;
    }

    public void GameOver(Player playerLose)
    {
        gameOverUIManager.gameObject.SetActive(true);
        State = GameState.GameOver;

        switch (playerLose)
        {
            case Player.Player1:
                player1.SetAnimation("Moody UnFriendly");
                player2.SetAnimation("Cheer Friendly");
                gameOverUIManager.UpdatePlayerWin(Player.Player2);
                break;
            case Player.Player2:
                player1.SetAnimation("Cheer Friendly");
                player2.SetAnimation("Moody UnFriendly");
                gameOverUIManager.UpdatePlayerWin(Player.Player1);
                break;
        }
    }
}
