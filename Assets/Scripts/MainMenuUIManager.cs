using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject howToPlayPanel;

    [SerializeField]
    private GameObject selectMode;

    public void HowToPlay()
    {
        howToPlayPanel.SetActive(true);
    }

    public void StartGame()
    {
        howToPlayPanel.SetActive(false);
        this.gameObject.SetActive(false);
        selectMode.SetActive(true);
    }
}
