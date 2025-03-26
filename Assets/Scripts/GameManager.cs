using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public class FloatArrayList
    {
        public List<float[]> records;
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt("words_typed_alltime", PlayerData.words_typed_alltime);
        PlayerPrefs.SetInt("coins", PlayerData.coins);
        PlayerPrefs.SetInt("factory_level", PlayerData.factory_level);
        PlayerPrefs.SetFloat("play_time", PlayerData.play_time);
        PlayerPrefs.SetInt("total_coins_alltime", PlayerData.total_coins_alltime);
        PlayerPrefs.SetInt("total_errors", PlayerData.total_errors);
        PlayerPrefs.SetInt("total_correct_chars", PlayerData.total_correct_chars);
        PlayerPrefs.SetInt("total_chars_typed", PlayerData.total_chars_typed);
        PlayerPrefs.SetFloat("top_wpm", PlayerData.top_wpm);
        
        foreach (var upgrade in PlayerData.upgrade_dict)
        {
            PlayerPrefs.SetInt("upgrade_" + upgrade.Key, upgrade.Value);
        }
        
        foreach (var bank in PlayerData.bank_dict)
        {
            PlayerPrefs.SetInt("bank_" + bank.Key, bank.Value);
        }
    }

    private void LoadGame()
    {
        PlayerData.words_typed_alltime = PlayerPrefs.GetInt("words_typed_alltime", 0);
        PlayerData.coins = PlayerPrefs.GetInt("coins", 0);
        PlayerData.factory_level = PlayerPrefs.GetInt("factory_level", 1);
        PlayerData.play_time = PlayerPrefs.GetFloat("play_time", 0f);
        PlayerData.total_coins_alltime = PlayerPrefs.GetInt("total_coins_alltime", 0);
        PlayerData.total_errors = PlayerPrefs.GetInt("total_errors", 0);
        PlayerData.total_correct_chars = PlayerPrefs.GetInt("total_correct_chars", 0);
        PlayerData.total_chars_typed = PlayerPrefs.GetInt("total_chars_typed", 0);
        PlayerData.top_wpm = PlayerPrefs.GetFloat("top_wpm", 0f);
        
        Dictionary<string, int> new_upgrade_dict = new Dictionary<string, int>();
        foreach (var key in PlayerData.upgrade_dict.Keys)
        {
            new_upgrade_dict[key] = PlayerPrefs.GetInt("upgrade_" + key, 0);
        }
        PlayerData.upgrade_dict = new_upgrade_dict;
        
        Dictionary<string, int> new_bank_dict = new Dictionary<string, int>();
        foreach (var key in PlayerData.bank_dict.Keys)
        {
            new_bank_dict[key] = PlayerPrefs.GetInt("bank_" + key, 0);
        }
        PlayerData.bank_dict = new_bank_dict;
    }

    public void ResetAllData()
    {
        PlayerData.words_typed_alltime = 0;
        PlayerData.coins = 0;
        PlayerData.factory_level = 1;
        PlayerData.play_time = 0f;
        PlayerData.total_coins_alltime = 0;
        PlayerData.total_errors = 0;
        PlayerData.total_correct_chars = 0;
        PlayerData.total_chars_typed = 0;
        PlayerData.top_wpm = 0;

        foreach (var key in new List<string>(PlayerData.upgrade_dict.Keys))
        {
            PlayerData.upgrade_dict[key] = 0;
        }

        foreach (var key in new List<string>(PlayerData.bank_dict.Keys))
        {
            PlayerData.bank_dict[key] = 0;
        }

        foreach (var key in new List<string>(PlayerData.achieve_dict.Keys))
        {
            PlayerData.achieve_dict[key] = false;
        }

        if (FactorySystem.extra_list != null)
        {
            FactorySystem.extra_list.Clear();
        }
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

    private void Update()
    {
        PlayerData.play_time += Time.deltaTime;

        PlayerData.UnlockAchievements();
    }
}
