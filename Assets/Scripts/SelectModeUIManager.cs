using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectModeUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainGame;

    [SerializeField]
    private GameObject selectDifficulty;

    public void OnePlayerMode()
    {
        GameManager.Instance.SetHasOnePlayer(true);
        this.gameObject.SetActive(false);
        selectDifficulty.SetActive(true);
    }

    public void TwoPlayerMode()
    {
        GameManager.Instance.StartGame();
        this.gameObject.SetActive(false);
        mainGame.SetActive(true);
    }
}
