using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem.Controls;

public class CurrencySystem : MonoBehaviour
{
    public TextMeshProUGUI currency_text;
    private float currency_timer;
    
    public void GainCurrency()
    {
        PlayerData.coins += PlayerData.upgrade_dict["coin_value"] + 5;
    }

    public void LoseCurrency()
    {
        PlayerData.coins -= 3 - PlayerData.upgrade_dict["error_deduction"];
    }

    private void UpdateText()
    {
        currency_text.text = $"Coins: {PlayerData.coins}";
    }

    private void GenerateCurrency()
    {
        if(PlayerData.upgrade_dict["coin_generator"] > 0)
        {
            PlayerData.coins += PlayerData.upgrade_dict["coin_generator"];
        }
    }

    void Update()
    {
        UpdateText();

        if(PlayerData.coins < 0)
        {
            PlayerData.coins = 0;
        }

        currency_timer += Time.deltaTime;
        if(currency_timer > 5f)
        {
            GenerateCurrency();
            currency_timer = 0;
        }
    }
}
