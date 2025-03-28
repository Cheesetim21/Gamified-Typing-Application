using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;
using JetBrains.Annotations;

public class TypingSystem : MonoBehaviour
{
    private StringBuilder typing_sentence = new StringBuilder();
    private StringBuilder trailing_sentence_A = new StringBuilder();
    private StringBuilder trailing_sentence_B = new StringBuilder();
    [SerializeField] private int cursor_position = 0;
    List<int> correct_chars_list = new List<int> {};
    List<int> total_chars_list = new List<int> {};
    List<int> sentence_char_correct_list = new List<int> {};
    private char space_char = '·';
    private char return_char = '↓';
    private float timer = 0f;
    private int correct_chars_a_second;
    private int total_chars_a_second;
    private int last_second = -1;
    private float WPM = 0f;
    private float accuracy = 100f;
    private bool timer_active = false;
    public Color correct_color = Color.gray;
    public Color incorrect_color = Color.red;
    public TextMeshProUGUI GUI_typing_text;
    public TextMeshProUGUI GUI_color_overlay_text;
    public TextMeshProUGUI GUI_WPM_text;
    public TextMeshProUGUI GUI_accuracy_text;
    public TextMeshProUGUI GUI_trail_sentence_a;
    public TextMeshProUGUI GUI_trail_sentence_b;
    public TextMeshProUGUI event_text;
    public TextMeshProUGUI CPS_text;
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));
    public AudioClip[] typing_sfx;
    private AudioSource audio_source;
    private CurrencySystem currency_system;
    private IngameEventSystem event_manager;
    List<int> coin_pos_array = new List<int> {};
    private System.Random rand = new System.Random();
    private bool is_error_pause = false;
    private float last_top_WPM_time = 0f;
    private float interval_top_WPM = 0f;
    private bool tracking_top_WPM = false;
    private int sentences_typed = 0;

    private void GetScripts()
    {
        GameObject currency = GameObject.Find("Currency Text");
        currency_system = currency.GetComponent<CurrencySystem>();

        GameObject events = GameObject.FindGameObjectWithTag("RandomEventManager");
        event_manager = events.GetComponent<IngameEventSystem>();
    }

    private void CoinPosGenerator(int coin_amt)
    {
        List<int> valid_positions = new List<int>();
        HashSet<int> coin_pos_set = new HashSet<int>(); // Ensures unique positions
        
        // Creates an array for all positions of a valid character - not whitespace
        for(int i=0; i < typing_sentence.Length; i++)
        {
            if (typing_sentence[i] != space_char && typing_sentence[i] != return_char)
            {
                valid_positions.Add(i);
            }
        }

        coin_amt = Mathf.Min(coin_amt, valid_positions.Count);
        System.Random rand = new System.Random();

        // Creates an array for positions of coin characters
        while (coin_pos_set.Count < coin_amt && coin_pos_set.Count < valid_positions.Count)
        {
            int random_index = rand.Next(valid_positions.Count);
            int random_pos = valid_positions[random_index];

            coin_pos_set.Add(random_pos);
        }

        coin_pos_array = coin_pos_set.ToList(); // Convert HashSet to List
    }

    private void CoinCharAwarder()
    {
        for (int i = 0; i < coin_pos_array.Count; i++)
        {
            if(cursor_position - 1 == coin_pos_array[i])
            {
                currency_system.GainCurrency();
                audio_source.PlayOneShot(typing_sfx[2]);
                break;
            }
        }

        audio_source.PlayOneShot(typing_sfx[0]);
    }

    private StringBuilder GenerateSentence(StringBuilder sentence)
    {
        string[] word_list = FactorySystem.GenerateWordList();

        // Randomly appends words until it fills the size of the text box
        while(sentence.Length < 27)
        {
            string word_to_add = word_list[UnityEngine.Random.Range(0, word_list.Length)];
            if(sentence.Length + word_to_add.Length <= 31)
            {
                sentence.Append(word_to_add + "·");
            }
        }

        sentence.Remove(sentence.Length - 1, 1);
        sentence.Append(return_char);
        sentence.Append(' ');
        
        return sentence;
    }


    private void SentenceUpdate()
    {
        // Scrolls sentences up by one, generating a new sentence for B
        typing_sentence = new StringBuilder(trailing_sentence_A.ToString()); 
        trailing_sentence_A = new StringBuilder(trailing_sentence_B.ToString()); 
        trailing_sentence_B = GenerateSentence(new StringBuilder()); 

        cursor_position = 0;
        sentence_char_correct_list.Clear();

        coin_pos_array.Clear();
        if(event_manager.gold_rush_active)
        {
            CoinPosGenerator(PlayerData.upgrade_dict["coin_frequency"] + 10);
        }
        else
        {
            CoinPosGenerator(PlayerData.upgrade_dict["coin_frequency"] + 2);
        }

        sentences_typed++;
        
    }


    private void KeyPressValidator()
    {
        // Code for validating key presses from: https://discussions.unity.com/t/find-out-which-key-was-pressed/616242/16
        char cursor_char = typing_sentence[cursor_position];

        // Checks which keycode corresponds if there is an input, then saves the keycode to a char
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode)) {
                    char char_typed = GetCharacterFromKeyCode(keyCode);
                    total_chars_a_second++;
                    PlayerData.total_chars_typed++;

                    if (!timer_active)
                    {
                        timer_active = true;
                    }

                    // Checks if character is correct, incorrect or invalid
                    if (char_typed == cursor_char)
                    { 
                        CharTypedCorrect(char_typed);
                    }
                    else if(char_typed == '\0')
                    {
                        break;
                    }
                    else if (!is_error_pause)
                    {
                        CharTypedIncorrect();
                    }
                }
            }
        }
    }


    // Converts KeyCode to character
    private char GetCharacterFromKeyCode(KeyCode keyCode)
    {
        // Maps all letter keys and space to their corresponding character and ignores all other inputs
        switch (keyCode)
        {
            case KeyCode.A: return 'a';
            case KeyCode.B: return 'b';
            case KeyCode.C: return 'c';
            case KeyCode.D: return 'd';
            case KeyCode.E: return 'e';
            case KeyCode.F: return 'f';
            case KeyCode.G: return 'g';
            case KeyCode.H: return 'h';
            case KeyCode.I: return 'i';
            case KeyCode.J: return 'j';
            case KeyCode.K: return 'k';
            case KeyCode.L: return 'l';
            case KeyCode.M: return 'm';
            case KeyCode.N: return 'n';
            case KeyCode.O: return 'o';
            case KeyCode.P: return 'p';
            case KeyCode.Q: return 'q';
            case KeyCode.R: return 'r';
            case KeyCode.S: return 's';
            case KeyCode.T: return 't';
            case KeyCode.U: return 'u';
            case KeyCode.V: return 'v';
            case KeyCode.W: return 'w';
            case KeyCode.X: return 'x';
            case KeyCode.Y: return 'y';
            case KeyCode.Z: return 'z';
            case KeyCode.Space: return space_char;
            case KeyCode.Return: return return_char;
            default: return '\0';
        }
    }


    private void CharTypedCorrect(char char_typed)
    {
        cursor_position++;
        correct_chars_a_second++;
        sentence_char_correct_list.Add(1);
        PlayerData.total_correct_chars++;

        if(char_typed == space_char || char_typed == return_char)
        {
            PlayerData.words_typed_alltime++;
            if(event_manager.precision_bonus_active && WasLastWordCorrect())
            {
                PlayerData.coins += 5;
                audio_source.PlayOneShot(typing_sfx[0]);
            }
        }
        
        CoinCharAwarder();
    }

    private bool WasLastWordCorrect()
    {
        if (cursor_position == 0) return false; 

        int start = cursor_position - 1;

        while (start > 0 && typing_sentence[start] != space_char && typing_sentence[start] != return_char)
        {
            start--;
        }

        if (typing_sentence[start] == space_char || typing_sentence[start] == return_char)
        {
            start++;
        }

        for (int i = start; i < cursor_position; i++)
        {
            if (sentence_char_correct_list[i] != 1) 
            {
                return false; 
            }
        }

        return true;
    }

    IEnumerator PauseAfterMistake()
    {
        is_error_pause = true;
        
        yield return new WaitForSeconds(0.2f);

        is_error_pause = false;
    }


    private void CharTypedIncorrect()
    {
        audio_source.PlayOneShot(typing_sfx[1]);

        sentence_char_correct_list.Add(0);
        cursor_position++;
        currency_system.LoseCurrency();
        PlayerData.total_errors++;

        StartCoroutine(PauseAfterMistake());
    }

    private void WPMCalculator()
    {
        if(sentences_typed < 3)
        {
            return;
        }

        float minutes_played = Mathf.Max(timer / 60f, 0.1f);
        
        // Caps WPM measurement at last 100 seconds of typing
        if(correct_chars_list.Count == 101)
        {
            correct_chars_list.RemoveAt(0);
        }

        if(timer > 100.0)
        {
            minutes_played = 1.6f;
        }

        // Calculates WPM using formula from 2.2.2
        float total_correct_chars = correct_chars_list.Sum();
        WPM = (total_correct_chars/5)/minutes_played;
        WPM = Mathf.Round(WPM * 10f) / 10f;
        
        if (WPM > PlayerData.top_wpm && !tracking_top_WPM && WPM < 170)
        {
            tracking_top_WPM = true;
            interval_top_WPM = WPM;
            last_top_WPM_time = PlayerData.play_time;
        }

        if (tracking_top_WPM)
        {
            interval_top_WPM = Mathf.Max(interval_top_WPM, WPM);

            if (timer - last_top_WPM_time >= 3.0f)
            {
                PlayerData.top_wpm = interval_top_WPM;
                Debug.Log(PlayerData.top_wpm);
                tracking_top_WPM = false;
            }
        }
    }


    private void AccuracyCalculator()
    {
        // Calculates accuracy using formula from 2.2.2
        float total_correct_chars = correct_chars_list.Sum();
        float total_chars_typed = total_chars_list.Sum();
        float errors = total_chars_typed - total_correct_chars;
        accuracy = ((total_chars_typed - errors)/total_chars_typed)*100;
        accuracy = Mathf.Round(accuracy * 10f) / 10f;
    }


    private void StatTimer()
    {
        timer += Time.deltaTime;
        int current_second = Mathf.FloorToInt(timer);
        
        //Calculates the WPM and accuracy every second 
        if (current_second > last_second)
        {
            correct_chars_list.Add(correct_chars_a_second);
            WPMCalculator();
            correct_chars_a_second = 0;

            total_chars_list.Add(total_chars_a_second);
            AccuracyCalculator();
            total_chars_a_second = 0;

            last_second = current_second;
        }
    }

    private string CharColorer(string display_text) // - from ChatGPT
    {
        string colored_text = "";

        // Loop through all characters in the display text
        for (int i = 0; i < display_text.Length; i++)
        {
            // Check if the character should be gold (yellow) while the cursor hasn't passed it
            bool isGold = coin_pos_array.Contains(i) && cursor_position <= i && !sentence_char_correct_list.Contains(i);

            // Default color is white
            string color_hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.white);

            // If it's a gold character, color it yellow (gold)
            if (isGold)
            {
                color_hex = UnityEngine.ColorUtility.ToHtmlStringRGB(Color.yellow);
            }
            // If the cursor has passed this character, check if it's typed correctly or incorrectly
            else if (i < cursor_position)
            {
                // Check if it's correctly typed (gray color for correct characters)
                color_hex = (sentence_char_correct_list[i] == 1)
                    ? UnityEngine.ColorUtility.ToHtmlStringRGB(correct_color)
                    : UnityEngine.ColorUtility.ToHtmlStringRGB(incorrect_color); // Red for incorrect characters
            }

            // Add the color to the character at position i
            colored_text += $"<color=#{color_hex}>{display_text[i]}</color>";
        }

        return colored_text;
    }


    private void UpdateTextDisplay()
    {
        string display_text = typing_sentence.ToString();

        // Updates the text to move the cursor
        string typing_text = display_text.Insert(cursor_position, "<u>");
        typing_text = typing_text.Insert(cursor_position + 4, "</u>"); 

        GUI_typing_text.text = typing_text;

        GUI_color_overlay_text.text = CharColorer(display_text);

        if(sentences_typed > 2)
        {
            GUI_WPM_text.text = $"WPM: {WPM.ToString()}";
        }
        else
        {
            GUI_WPM_text.text = $"WPM: N/A";
        }

        GUI_accuracy_text.text = $"Accuracy: {accuracy.ToString()}%";

        GUI_trail_sentence_a.text = trailing_sentence_A.ToString();
        GUI_trail_sentence_b.text = trailing_sentence_B.ToString();

        if(PlayerData.upgrade_dict["coin_generator"] > 0)
        {
            CPS_text.gameObject.SetActive(true);
            CPS_text.text = $"{PlayerData.upgrade_dict["coin_generator"]} coins / 5 sec";
        }

        if(event_manager.gold_rush_active)
        {
            event_text.text = "Gold Rush!!! +8 gold characters";
        }
        else if(event_manager.precision_bonus_active)
        {
            event_text.text = "Precision Bonus!!! +5 coins per correctly typed word";
        }
        else
        {
            event_text.text = "";
        }
    }


    void Awake()
    {
        typing_sentence = GenerateSentence(new StringBuilder());
        trailing_sentence_A = GenerateSentence(new StringBuilder());
        trailing_sentence_B = GenerateSentence(new StringBuilder());
        audio_source = GetComponent<AudioSource>();

        GetScripts();
        if(event_manager.gold_rush_active)
        {
            CoinPosGenerator(PlayerData.upgrade_dict["coin_frequency"] + 10);
        }
        else
        {
            CoinPosGenerator(PlayerData.upgrade_dict["coin_frequency"] + 2);
        }
        CPS_text.gameObject.SetActive(false);
    }


    void Update()
    {
        UpdateTextDisplay();

        if(cursor_position != typing_sentence.Length - 1)
        {
            KeyPressValidator();
        }
        else
        {
            SentenceUpdate();
        }

        if(timer_active)
        {
            StatTimer();
        }
    }
}
