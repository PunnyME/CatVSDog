using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameUIManager : MonoBehaviour
{
    [SerializeField]
    private Slider windLeft;

    [SerializeField]
    private Slider windRight;

    [SerializeField]
    private TextMeshProUGUI playerTurnText;

    public void UpdateWind(float wind)
    {
        if (wind < 0)
        {
            windLeft.value = wind * -1f / 5f;
            windRight.value = 0;
        }
        else if (wind > 0)
        {
            windRight.value = wind/ 5f;
            windLeft.value = 0;
        }
        else
        {   
            windLeft.value = 0;
            windRight.value = 0;
        }
    }

    public void UpdatePlayerTurn()
    {
        switch (GameManager.Instance.PlayerTurn)
        {
            case PlayerTurn.Player1Turn:
                playerTurnText.text = "Player 1";
                break;
            case PlayerTurn.Player2Turn:
                playerTurnText.text = "Player 2";
                break;
        }
    }
}
