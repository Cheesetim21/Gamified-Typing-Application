using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void SaveGame()
    {
        PlayerPrefs.SetInt("coins", PlayerData.coins);
    }

    private void LoadGame()
    {
        //PlayerData.coins = PlayerPrefs.GetInt("coins");
        PlayerData.ResetAllData();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
            InvokeRepeating(nameof(SaveGame), 30f, 30f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
