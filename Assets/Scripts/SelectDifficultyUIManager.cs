using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDifficultyUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainGame;

    public void EasyMode()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Easy);
        GameManager.Instance.StartGame();
        this.gameObject.SetActive(false);
        mainGame.SetActive(true);
    }

    public void NormalMode()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Normal);
        GameManager.Instance.StartGame();
        this.gameObject.SetActive(false);
        mainGame.SetActive(true);
    }

    public void HardMode()
    {
        GameManager.Instance.SetDifficulty(Difficulty.Hard);
        GameManager.Instance.StartGame();
        this.gameObject.SetActive(false);
        mainGame.SetActive(true);
    }
}
