
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.MPE;

public class IngameEventSystem : MonoBehaviour
{
    private static IngameEventSystem instance;
    private bool cooldown_active = true;
    public bool gold_rush_active = false;
    public bool precision_bonus_active = false;
    private float cooldown_duration = 180f;
    private float event_duration = 30f;

    private IEnumerator EventCooldown()
    {
        cooldown_active = true;
        yield return new WaitForSeconds(cooldown_duration); 
        cooldown_active = false;
    }

    private IEnumerator EventDuration(string chosen_event)
    {
        if (chosen_event == "Gold Rush")
        {
            gold_rush_active = true;
        }
        else
        {
            precision_bonus_active = true;
        }

        yield return new WaitForSeconds(event_duration); 

        gold_rush_active = false;
        precision_bonus_active = false;

        StartCoroutine(EventCooldown());
    }

    private void StartEvent(string chosen_event)
    {
        StartCoroutine(EventDuration(chosen_event));
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return; 
        }
    }

    void Start()
    {

        StartCoroutine(EventCooldown());
    }


    void Update()
    {
        if(!cooldown_active && !gold_rush_active && !precision_bonus_active)
        {
            bool goldRushUnlocked = PlayerData.upgrade_dict["gold_rush"] == 1;
            bool precisionBonusUnlocked = PlayerData.upgrade_dict["precision_bonus"] == 1;
            string random_event = null;

            if (goldRushUnlocked && precisionBonusUnlocked)
            {
                random_event = Random.value < 0.5f ? "Gold Rush" : "Precision Bonus";
            }
            else if (goldRushUnlocked)
            {
                random_event = "Gold Rush";
            }
            else if (precisionBonusUnlocked)
            {
                random_event = "Precision Bonus";
            }

            if(random_event != null)
            {
                StartEvent(random_event);
            }
        }
    }
}
