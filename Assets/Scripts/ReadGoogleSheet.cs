using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadGoogleSheet : MonoBehaviour
{
    [System.Serializable]
    public class GameData
    {
        public string Name;
        public string Amount;
        public string Damage;
        public string HP;
        public string MissingChance;
        public string Sec;
        public string WindForce;
    }

    [System.Serializable]
    public class GameDataList
    {
        public GameData[] Data;
    }

    public GameDataList _GameDataList;

    private string json;
    
    void Awake()
    {
        StartCoroutine(ObtainSheetData());
    }

    IEnumerator ObtainSheetData()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://sheetdb.io/api/v1/mnbcgdzutrzwp");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("Error: " +  request.error);
        }
        else
        {
            json = request.downloadHandler.text;
            json = "{\"Data\":" + json + "}";
            _GameDataList = JsonUtility.FromJson<GameDataList>(json);
        }
    }
}
