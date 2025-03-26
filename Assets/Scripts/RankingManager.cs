using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI company_ranking_text;
    public TextMeshProUGUI level_ranking_text;

    private class Company
    {
        public string name;
        public int level;

        public Company(string name, int level)
        {
            this.name = name;
            this.level = level;
        }
    }

    private List<Company> company_leaderboard;

    private void UpdateLeaderboard()
    {
        foreach(Company company in company_leaderboard)
        {
            if (company.name == "You")
            {
                company.level = PlayerData.factory_level;
            }
        }
        
        company_leaderboard.Sort((a,b) => b.level.CompareTo(a.level));
    }

    private void DisplayLeaderboard()
    {
        StringBuilder company_name_text = new StringBuilder();
        StringBuilder company_level_text = new StringBuilder();

        for (int i = 0; i < company_leaderboard.Count; i++)
        {
            company_name_text.AppendLine(company_leaderboard[i].name);
            company_level_text.AppendLine($"Factory Lv. {company_leaderboard[i].level}");
        }

        company_ranking_text.text = company_name_text.ToString();
        level_ranking_text.text = company_level_text.ToString();
    }
   
    void Start()
    {
        company_leaderboard = new List<Company>
        {
            new Company("Ctrl Alt Elite", 9),
            new Company("Worda Cola", 8),
            new Company("Peps-key", 6),
            new Company("TypoBell", 5),
            new Company("ClickDonalds", 4),
            new Company("WPMmart", 3),
            new Company("KeyFC", 2),
            new Company("You", PlayerData.factory_level)
        };

        UpdateLeaderboard();
        DisplayLeaderboard();
    }


}
