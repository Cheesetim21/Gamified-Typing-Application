using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AchievementsManager : MonoBehaviour
{
    public UnityEngine.UI.Image[] trophy_sprites;

    private void UpdateAchievements()
    {
        int index = 0;

        foreach (UnityEngine.UI.Image trophy in trophy_sprites)
        {
            var entry = PlayerData.achieve_dict.ElementAt(index);
            bool unlocked = entry.Value;

            if(!unlocked)
            {
                trophy.color = Color.black;
            }
            
            index++;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateAchievements();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerData.UnlockAchievements();
        UpdateAchievements();
    }
}
