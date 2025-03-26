using System.Linq;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public TextMeshProUGUI playtime_text;
    public TextMeshProUGUI words_typed_text;
    public TextMeshProUGUI top_WPM_text;
    public TextMeshProUGUI total_coins_text;
    public TextMeshProUGUI total_acc_text;

    private string ConvertPlaytime(float playtime)
    {
        int hours = Mathf.FloorToInt(playtime / 3600);
        int minutes = Mathf.FloorToInt((playtime % 3600) / 60);
        int seconds = Mathf.FloorToInt(playtime % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    private float TotalAccuracyCalculator()
    {
        float accuracy = ((float)PlayerData.total_correct_chars - PlayerData.total_errors)/PlayerData.total_chars_typed * 100;
        accuracy = Mathf.Round(accuracy * 10f) / 10f;
        return accuracy;
    }

    private void UpdateStats()
    {
        playtime_text.text = $"Playtime: {ConvertPlaytime(PlayerData.play_time)}";
        words_typed_text.text = $"Words Typed: {PlayerData.words_typed_alltime}";
        total_coins_text.text = $"Total Coins: {PlayerData.total_coins_alltime}";
        total_acc_text.text = $"Total Acc: {TotalAccuracyCalculator()}%";
        top_WPM_text.text = $"Top WPM: {PlayerData.top_wpm}";
    }

    private void Update()
    {
        UpdateStats();
    }
}
