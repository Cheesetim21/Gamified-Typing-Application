using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

public class FactorySystem : MonoBehaviour
{
    private int[] word_bank_prices = {50, 100, 200, 350, 600, 900, 1500, 2500, 4000, 6000};
    public TextMeshProUGUI factory_button_text;
    public TextMeshProUGUI unlock_bank_text;
    public static List<string> extra_list = new List<string>();
    public AudioClip[] factory_sfx;
    private AudioSource audio_source;
    public Sprite [] sprite_array;
    private SpriteRenderer sprite_renderer;

    public static string[] GenerateWordList()
    {
        string file_path = Path.Combine(Application.streamingAssetsPath, "Default.txt");
        List<string> word_list = new List<string>();

        // Reads word list from the text file and splits into array 
        using(StreamReader streamReader = new StreamReader(file_path))
        {
            string text_contents = streamReader.ReadToEnd();
            word_list.AddRange(text_contents.Split(new char[] {' ', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries));
        }

        word_list.AddRange(extra_list);

        return word_list.ToArray();
    }

    private void UnlockWordBank(string word_bank)
    {
        string file_path = Path.Combine(Application.streamingAssetsPath, word_bank + ".txt");

        using (StreamReader streamReader = new StreamReader(file_path))
        {
            string text_contents = streamReader.ReadToEnd();
            extra_list.AddRange(text_contents.Split(new char[] {' ', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries));
        }
    }

    public void PurchaseWordBank()
    {
        var locked_banks = new Dictionary<string, int>();

        foreach (var item in PlayerData.bank_dict)
        {
            if (item.Value != 1)
            {
                locked_banks.Add(item.Key, item.Value);
            }
        }

        if (locked_banks.Count > 0)
        {
            if (PlayerData.coins > word_bank_prices[PlayerData.factory_level - 1])
            {
                PlayerData.coins -= word_bank_prices[PlayerData.factory_level - 1];
                
                System.Random rand = new System.Random();
                int random_index = rand.Next(locked_banks.Count);
                string random_key = new List<string>(locked_banks.Keys)[random_index];
                PlayerData.bank_dict[random_key] = 1;
                UnlockWordBank(random_key);

                PlayerData.factory_level++;
                audio_source.PlayOneShot(factory_sfx[0]);
                StartCoroutine(ShowBankUnlock(random_key));
            }
            else
            {
                audio_source.PlayOneShot(factory_sfx[1]);
            }
        }
        else
        {
            audio_source.PlayOneShot(factory_sfx[1]);
        }

        UpdateButtonText();
        UpdateFactorySprite();
    }

    private void UpdateButtonText()
    {
        if(PlayerData.factory_level != 10)
        {
            factory_button_text.text = $"Factory [Lv. {PlayerData.factory_level}]\nCoins: {word_bank_prices[PlayerData.factory_level - 1]}";
        }
        else
        {
            factory_button_text.text = "FULLY UPGRADED";
        }
    }

    private void UpdateFactorySprite()
    {
        sprite_renderer.sprite = sprite_array[PlayerData.factory_level];
    }

    private IEnumerator ShowBankUnlock(string bank_name)
    {
        unlock_bank_text.text = $"{bank_name.Replace("_", " ")} Words Unlocked!";
        SetAlpha(1f);
        yield return new WaitForSeconds(5); 
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = unlock_bank_text.color;
        color.a = Mathf.Clamp01(alpha); 
        unlock_bank_text.color = color;
    }
    
    void Start()
    {
        foreach (var item in PlayerData.bank_dict)
        {
            if (item.Value == 1)
            {
                UnlockWordBank(item.Key);
            }
        }

        audio_source = GetComponent<AudioSource>();
        sprite_renderer = GetComponent<SpriteRenderer>();

        UpdateButtonText();
        UpdateFactorySprite();
        SetAlpha(0f);
    }
}
