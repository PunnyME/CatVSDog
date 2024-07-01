using Spine.Unity;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player Player => player;

    [SerializeField]
    private Player player;

    [SerializeField]
    private Slider HPBar;

    [SerializeField]
    private Slider forceCharge;

    [SerializeField]
    private Slider timeCount;

    [SerializeField] 
    private SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private ThrowItem normalThrow;

    [SerializeField]
    private ThrowItem powerThrow;

    [SerializeField]
    private Transform enemy;

    [SerializeField]
    private Transform throwStart;

    [SerializeField]
    private Transform throwDestination;

    [SerializeField]
    private Transform initialThrowDestination;

    [SerializeField]
    private float initialForce = 15f;

    private float playerHP;
    private float initialPlayerHP;

    private float missingChance;

    private PlayerTurn playerTurn;
    private float force;

    private bool isThrowing;
    private bool isDoubleThrow;
    private ThrowItem throwItem;

    private float throwDestinationX;

    public void Init()
    {
        switch (player)
        {
            case Player.Player1:
                playerTurn = PlayerTurn.Player1Turn;
                break;
            case Player.Player2:
                playerTurn = PlayerTurn.Player2Turn;
                break;
        }

        if (GameManager.Instance.HasOnePlayer && player == Player.Player2)
        {
            string playerHPStr = "";
            string missingChanceStr = "";

            switch (GameManager.Instance.Difficulty)
            {
                case Difficulty.Easy:
                    playerHPStr = GameManager.Instance.GameData._GameDataList.Data[1].HP;
                    missingChanceStr = GameManager.Instance.GameData._GameDataList.Data[1].MissingChance;
                    break;
                case Difficulty.Normal:
                    playerHPStr = GameManager.Instance.GameData._GameDataList.Data[2].HP;
                    missingChanceStr = GameManager.Instance.GameData._GameDataList.Data[2].MissingChance;
                    break;
                case Difficulty.Hard:
                    playerHPStr = GameManager.Instance.GameData._GameDataList.Data[3].HP;
                    missingChanceStr = GameManager.Instance.GameData._GameDataList.Data[3].MissingChance;
                    break;
            }

            playerHP = float.Parse(playerHPStr);
            missingChanceStr = missingChanceStr.Replace("%", "");
            missingChance = float.Parse(missingChanceStr);
        }
        else
        {
            playerHP = float.Parse(GameManager.Instance.GameData._GameDataList.Data[0].HP);
        }

        initialPlayerHP = playerHP;
        Reset();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (GameManager.Instance.State == GameState.Playing && GameManager.Instance.PlayerTurn == playerTurn)
        {
            isThrowing = true;
            GameManager.Instance.SetIsThrowing(true);
            forceCharge.gameObject.SetActive(true);
            throwDestination.position = new Vector3(initialThrowDestination.position.x, initialThrowDestination.position.y, initialThrowDestination.position.z);
            force = 0f;
            SetAnimation("Idle Friendly 1");
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (GameManager.Instance.State == GameState.Playing && GameManager.Instance.PlayerTurn == playerTurn)
        {
            isThrowing = false;
            GameManager.Instance.SetIsThrowing(false);
            forceCharge.gameObject.SetActive(false);
            Throw(throwItem);
            SetAnimation("Idle UnFriendly 1");
            GameManager.Instance.ChangeGameState(GameState.Processing);
        }
    }

    public bool IsPlayerTurn()
    {
        if (GameManager.Instance.PlayerTurn == playerTurn)
        {
            return true;
        }

        return false;
    }

    public void SetThrowItem(bool isPowerThrow = false)
    {
        if (!isPowerThrow)
        {
            throwItem = normalThrow;
        }
        else
        {
            throwItem = powerThrow;
        }
    }

    public void AIThrow()
    {
        throwDestination.position = new Vector3(initialThrowDestination.position.x, initialThrowDestination.position.y, initialThrowDestination.position.z);
        force = 0f;
        float windForce;

        switch (GameManager.Instance.Difficulty)
        {
            case Difficulty.Easy:
                Throw(normalThrow);
                break;
            case Difficulty.Normal:
                windForce = float.Parse(GameManager.Instance.GameData._GameDataList.Data[11].WindForce);
                if (Mathf.Abs(GameManager.Instance.Wind) < windForce)
                {
                    isDoubleThrow = true;
                    Throw(powerThrow);
                }
                else
                {
                    Throw(normalThrow);
                }
                break;
            case Difficulty.Hard:
                windForce = float.Parse(GameManager.Instance.GameData._GameDataList.Data[12].WindForce);
                if (Mathf.Abs(GameManager.Instance.Wind) > windForce)
                {
                    Throw(powerThrow);
                }
                else
                {
                    Throw(normalThrow);
                }
                break;
        }
        
        GameManager.Instance.ChangeGameState(GameState.Processing);
    }

    private void Throw(ThrowItem throwItem)
    {
        if (isDoubleThrow)
        {
            StartCoroutine(DoubleThrow(throwItem));
        }
        else
        {
            NormalThrow(throwItem);
        }
    }

    private void NormalThrow(ThrowItem throwItem, bool isDoubleThrow = false)
    {
        ThrowItem item = Instantiate(throwItem, throwStart.position, throwStart.rotation);
        float wind = GameManager.Instance.Wind;
        throwDestinationX = throwDestination.position.x;

        if (player == Player.Player2)
        {
            if (GameManager.Instance.HasOnePlayer)
            {
                float missingChanceFlt = Random.Range(0f, 100f);

                if (missingChanceFlt > missingChance)
                {
                    throwDestination.position = new Vector3(enemy.position.x, enemy.position.y, enemy.position.z);
                }
                else
                {
                    throwDestination.position = new Vector3(throwDestination.position.x + initialForce + wind, throwDestination.position.y, throwDestination.position.z);
                }
            }
            else
            {
                if (throwDestination.position.x + force + wind < throwDestinationX)
                {
                    throwDestination.position = new Vector3(throwDestination.position.x, throwDestination.position.y, throwDestination.position.z);
                }
                else
                {
                    throwDestination.position = new Vector3(throwDestination.position.x + force + wind, throwDestination.position.y, throwDestination.position.z);
                }
            }
        }
        else
        {
            if (throwDestination.position.x - force + wind > throwDestinationX)
            {
                throwDestination.position = new Vector3(throwDestination.position.x, throwDestination.position.y, throwDestination.position.z);
            }
            else
            {
                throwDestination.position = new Vector3(throwDestination.position.x - force + wind, throwDestination.position.y, throwDestination.position.z);
            }
        }

        item.Init(throwStart, throwDestination, player, isDoubleThrow);
    }

    private IEnumerator DoubleThrow(ThrowItem throwItem)
    {
        int throwAmount = int.Parse(GameManager.Instance.GameData._GameDataList.Data[7].Amount);
        for (int i = 0; i < throwAmount - 1; i++)
        {
            NormalThrow(throwItem, true);
            yield return new WaitForSeconds(2f);
            throwDestination.position = new Vector3(initialThrowDestination.position.x, initialThrowDestination.position.y, initialThrowDestination.position.z);
        }
        NormalThrow(throwItem);
    }

    public void SetIsDoubleThrow(bool isDoubleThrow)
    {
        this.isDoubleThrow = isDoubleThrow;
    }

    public void DealDamage(float damage)
    {
        playerHP -= damage;

        if (playerHP <= 0f)
        {
            playerHP = 0f;
            GameManager.Instance.GameOver(player);
        }

        HPBar.value = playerHP / initialPlayerHP;
    }

    public void Heal(float point)
    {
        playerHP += point;

        if (playerHP > initialPlayerHP)
        {
            playerHP = initialPlayerHP;
        }

        HPBar.value = playerHP / initialPlayerHP;
    }

    public void SetAnimation(string animationName)
    {
        skeletonAnimation.state.SetAnimation(0, animationName, true);
    }

    public void SetTimeCount(bool active, float time)
    {
        timeCount.gameObject.SetActive(active);
        timeCount.value = time;
    }

    public void ResetTurn()
    {
        SetThrowItem();
        SetIsDoubleThrow(false);
        timeCount.gameObject.SetActive(false);
        SetAnimation("Idle UnFriendly 1");
    }

    public void Reset()
    {
        playerHP = initialPlayerHP;
        HPBar.value = 1f;
        SetAnimation("Idle UnFriendly 1");
        ResetTurn();
    }

    void Update()
    {
        if (isThrowing)
        {
            if (force >= 0 && force <= initialForce)
            {
                force += Time.deltaTime * 10f;
            }

            forceCharge.value = force / initialForce;
        }
    }
}
