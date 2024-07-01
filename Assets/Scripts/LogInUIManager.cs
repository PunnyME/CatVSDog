using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogInUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu;

    public void GuesteLogIn()
    {
        MainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
