using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialItemManager : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter player;

    [SerializeField]
    private GameObject PowerThrowButton;

    [SerializeField]
    private GameObject DoubleThrowButton;

    [SerializeField]
    private GameObject HealButton;

    void Start()
    {
        if (GameManager.Instance.HasOnePlayer && player.Player == Player.Player2)
        {
            PowerThrowButton.SetActive(false);
            DoubleThrowButton.SetActive(false);
            HealButton.SetActive(false);
        }
    }

    public void PowerThrow()
    {
        if (player.IsPlayerTurn())
        {
            PowerThrowButton.SetActive(false);
            player.SetThrowItem(true);
        }
    }

    public void DoubleThrow()
    {
        if (player.IsPlayerTurn())
        {
            DoubleThrowButton.SetActive(false);
            player.SetIsDoubleThrow(true);
        }
    }

    public void Heal()
    {
        if (player.IsPlayerTurn())
        {
            player.Heal(float.Parse(GameManager.Instance.GameData._GameDataList.Data[8].HP));
            GameManager.Instance.ChangeGameState(GameState.Processing);
            HealButton.SetActive(false);
            GameManager.Instance.TurnEnd();
        }
    }

    public void Reset()
    {
        PowerThrowButton.SetActive(true);        
        DoubleThrowButton.SetActive(true);        
        HealButton.SetActive(true);
    }
}
