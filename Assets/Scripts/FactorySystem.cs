using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class FactorySystem : MonoBehaviour
{
    private int[] word_bank_prices = {100, 200, 350, 600, 1000, 1500, 2500, 4000, 7500, 10000};
    public TextMeshProUGUI factory_button_text;
    private static List<string> extra_list = new List<string>();

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
            System.Random rand = new System.Random();
            int random_index = rand.Next(locked_banks.Count);
            string random_key = new List<string>(locked_banks.Keys)[random_index];

            PlayerData.bank_dict[random_key] = 1;
            UnlockWordBank(random_key);
        }
        else
        {
            factory_button_text.text = "FULLY UPGRADED";
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
