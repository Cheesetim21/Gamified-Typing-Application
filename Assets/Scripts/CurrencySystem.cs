using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CurrencySystem : MonoBehaviour
{
    public TextMeshProUGUI currency_text;
    
    
    

    public void GainCurrency()
    {
        PlayerData.coins += 5;
    }

    public void LoseCurrency()
    {
        PlayerData.coins -= 2;
    }

    private void UpdateText()
    {
        currency_text.text = $"Coins: {PlayerData.coins}";
    }

    void Start()
    {

    }


    void Update()
    {
        UpdateText();

        if(PlayerData.coins < 0)
        {
            PlayerData.coins = 0;
        }
    }
}
