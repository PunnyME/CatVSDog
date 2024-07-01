using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerWinText;

    public void UpdatePlayerWin(Player player)
    {
        switch (player)
        {
            case Player.Player1:
                playerWinText.text = "Player 1 Win";
                break;
            case Player.Player2:
                playerWinText.text = "Player 2 Win";
                break;
        }
    }

    public void Replay()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void Share()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile( filePath ).Share();
    }
}
