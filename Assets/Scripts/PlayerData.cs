using UnityEngine;
using System;
using System.Collections.Generic;

public static class PlayerData
{
    public static int words_typed_alltime = 0;

    public static int coins = 0;

    public static Dictionary<string, int> upgrade_dict = new Dictionary<string, int>
    {
        { "coin_value", 0},
        { "coin_frequency", 0},
        { "coin_generator", 0},
        { "error_deduction", 0},
        { "gold_rush", 0},
        { "precision_bonus", 0},
        { "statistics", 0},
        { "rankings", 0}
    };

    // Reset all static data
    public static void ResetAllData()
    {
        words_typed_alltime = 0;
        coins = 0;

        foreach (var key in new List<string>(upgrade_dict.Keys))
        {
            upgrade_dict[key] = 0;
        }
    }
}
