using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    private Dictionary<string, int[]> upgrade_prices = new Dictionary<string, int[]>
    {
        { "coin_value", new int[] { 250, 750, 1750, 3500, 6000, 0 } },
        { "coin_frequency", new int[] { 500, 1500, 4000, 7500, 12000, 0 } },
        { "coin_generator", new int[] { 750, 2000, 4500, 9000, 15000, 0 } },
        { "error_deduction", new int[] { 1000, 4000, 0 } },
        { "gold_rush", new int[] { 8000, 0 } },
        { "precision_bonus", new int[] { 3000, 0 } },
        { "statistics", new int[] { 1000, 0 } },
        { "rankings", new int[] { 3500, 0 } }
    };

    public TextMeshProUGUI[] upgrade_texts;
    private Dictionary<string, bool> upgrade_maxxed = new Dictionary<string, bool>();

    public AudioClip[] shop_sfx;
    private AudioSource audio_source;

    public void UpdateUpgradeTexts()
    {
        int index = 0;
        foreach (var upgrade in upgrade_prices)
        {
            int level = PlayerData.upgrade_dict[upgrade.Key];
            int price = upgrade.Value[level];

            if (price == 0)
            {
                upgrade_maxxed[upgrade.Key] = true;
                upgrade_texts[index].text = "SOLD OUT";
            }
            else
            {
                upgrade_texts[index].text = $"{upgrade.Key.Replace("_", " ").ToUpper()}\nCoins: {price}";
            }
            index++;
        }
    }

    public void BuyUpgrade(string upgrade_key)
    {
        int level = PlayerData.upgrade_dict[upgrade_key];
        int price = upgrade_prices[upgrade_key][level];

        if (upgrade_maxxed[upgrade_key] || PlayerData.coins < price)
        {
            audio_source.PlayOneShot(shop_sfx[1]);
            return;
        }
        else
        {
            audio_source.PlayOneShot(shop_sfx[0]);
            PlayerData.coins -= price;
            PlayerData.upgrade_dict[upgrade_key] = level + 1;
            UpdateUpgradeTexts();
        }
    }

    void Start()
    {
        foreach (var upgrade in upgrade_prices)
        {
            upgrade_maxxed[upgrade.Key] = false;
        }

        UpdateUpgradeTexts();
        audio_source = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F10))
        {
            PlayerData.coins += 1000;
        }
    }
}
