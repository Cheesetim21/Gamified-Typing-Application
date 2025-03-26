using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using Mono.Cecil;

public static class PlayerData
{
    public static int coins = 0;
    public static int total_coins_alltime = 0;
    public static int factory_level = 1;
    public static float play_time = 0f;
    public static int words_typed_alltime = 0;
    public static int total_errors = 0;
    public static int total_correct_chars = 0;
    public static int total_chars_typed = 0;
    public static float top_wpm = 0;

    public static Dictionary<string, int> upgrade_dict = new Dictionary<string, int>
    {
        { "coin_value", 0},
        { "coin_frequency", 0},
        { "coin_generator", 0},
        { "error_deduction", 0},
        { "gold_rush", 0},
        { "precision_bonus", 0},
        { "rankings", 0},
        { "achievements", 0}
    };

    public static Dictionary<string, int> bank_dict = new Dictionary<string, int>
    {
        { "4_Letter", 0},
        { "5_Letter", 0},
        { "6_Letter", 0},
        { "7_Letter", 0},
        { "8_Letter", 0},
        { "Animal_Kingdom", 0},
        { "World_Countries", 0},
        { "Movie_Titles", 0},
        { "Food_and_Drink", 0}
    };

    public static Dictionary<string, bool> achieve_dict = new Dictionary<string, bool>
    {
        {"1000 Total Coins", false},
        {"10K Total Coins", false},
        {"100K Total Coins", false},
        {"500 Words Typed", false},
        {"5K Words Typed", false},
        {"20K Words Typed", false},
        {"1K Errors Typed", false},
        {"All Upgrades Bought", false},
        {"All Upgrades Maxxed", false},
        {"Factory Upgrades Maxxed", false},
        {"1 Hour Playtime", false},
        {"3 Hours Playtime", false},
        {"5 Hours Playtime", false},
        {"Reach WPM Above 100", false},
        {"Complete the Game", false}
    };

    public static bool CheckUpgradesMaxxed()
    {
        var max_upg_checks = new Dictionary<string, int>
        {
            { "coin_value", 5},
            { "coin_frequency", 5},
            { "coin_generator", 5},
            { "error_deduction", 2},
        };

        foreach (var max_check in max_upg_checks)
        {
            if(upgrade_dict.TryGetValue(max_check.Key, out int level) && level != max_check.Value)
            {
                return false;
            }
        }

        return true;
    }

    public static void UnlockAchievements()
    {
        if(total_coins_alltime >= 1000 & !achieve_dict["1000 Total Coins"])
        {
            achieve_dict["1000 Total Coins"] = true;
        }

        if(total_coins_alltime >= 10000 & !achieve_dict["10K Total Coins"])
        {
            achieve_dict["10K Total Coins"] = true;
        }

        if(total_coins_alltime >= 100000 & !achieve_dict["100K Total Coins"])
        {
            achieve_dict["100K Total Coins"] = true;
        }

        if(words_typed_alltime >= 500 & !achieve_dict["500 Words Typed"])
        {
            achieve_dict["500 Words Typed"] = true;
        }

        if(words_typed_alltime >= 5000 & !achieve_dict["5K Words Typed"])
        {
            achieve_dict["5K Words Typed"] = true;
        }

        if(words_typed_alltime >= 20000 & !achieve_dict["20K Words Typed"])
        {
            achieve_dict["20K Words Typed"] = true;
        }

        if(play_time/3600f > 1 & !achieve_dict["1 Hour Playtime"])
        {
            achieve_dict["1 Hour Playtime"] = true;
        }

        if(play_time/3600f > 3 & !achieve_dict["3 Hours Playtime"])
        {
            achieve_dict["3 Hours Playtime"] = true;
        }

        if(play_time/3600f > 5 & !achieve_dict["5 Hours Playtime"])
        {
            achieve_dict["5 Hours Playtime"] = true;
        }

        if(top_wpm > 100 & !achieve_dict["Reach WPM Above 100"])
        {
            achieve_dict["Reach WPM Above 100"] = true;
        }

        if(total_errors >= 1000 & !achieve_dict["1K Errors Typed"])
        {
            achieve_dict["1K Errors Typed"] = true;
        } 

        if(factory_level == 10 & !achieve_dict["Factory Upgrades Maxxed"])
        {
            achieve_dict["Factory Upgrades Maxxed"] = true;
        }

        if(CheckUpgradesMaxxed() & !achieve_dict["All Upgrades Maxxed"])
        {
            achieve_dict["All Upgrades Maxxed"] = true;
        }

        bool all_bought = upgrade_dict.Values.All(value => value != 0);

        if(all_bought & !achieve_dict["All Upgrades Bought"])
        {
            achieve_dict["All Upgrades Bought"] = true;
        }

        bool all_achieve = achieve_dict.Values.Count(value => value == false) <= 1;

        if(all_achieve & !achieve_dict["Complete the Game"])
        {
            achieve_dict["Complete the Game"] = true;
        }
    }
}
