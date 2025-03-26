using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShopSystem : MonoBehaviour
{
    private Dictionary<string, int[]> upgrade_prices = new Dictionary<string, int[]>
    {
        { "coin_value", new int[] { 100, 300, 800, 1500, 2500, 0 } },
        { "coin_frequency", new int[] { 200, 500, 1200, 2000, 3500, 0 } },
        { "coin_generator", new int[] { 300, 750, 1300, 2000, 3200, 0 } },
        { "error_deduction", new int[] { 400, 1200, 0 } },
        { "gold_rush", new int[] { 2000, 0 } },
        { "precision_bonus", new int[] { 1000, 0 } },
        { "rankings", new int[] { 250, 0 } },
        { "achievements", new int[] { 500, 0 } }
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
                string upgrade_caption = $"{upgrade.Key.Replace("_", " ").ToUpper()}";

                var leveled_upgrades = PlayerData.upgrade_dict.Keys.Take(4);

                if(leveled_upgrades.Contains(upgrade.Key))
                {
                    upgrade_caption = upgrade_caption + $" [Lv. {level + 1}]";
                }

                upgrade_texts[index].text = upgrade_caption + $"\nCoins: {price}";
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
}
