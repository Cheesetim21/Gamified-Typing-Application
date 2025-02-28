using TMPro;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{

    public TextMeshProUGUI currency_text;
    
    public void GainCurrency()
    {
        PlayerData.coins++;
    }

    public void LoseCurrency()
    {
        PlayerData.coins = PlayerData.coins - 5;
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
