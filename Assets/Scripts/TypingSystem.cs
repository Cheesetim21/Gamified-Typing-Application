using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections.Generic;

public class TypingSystem : MonoBehaviour
{
    private string[] word_list;
    private StringBuilder typing_sentence = new StringBuilder();
    private StringBuilder trailing_sentence_A = new StringBuilder();
    private StringBuilder trailing_sentence_B = new StringBuilder();
    [SerializeField] private int cursor_position = 0;
    List<int> correct_chars_list = new List<int> {};
    List<int> total_chars_list = new List<int> {};
    private float timer = 0f;
    private int correct_chars_a_second;
    private int total_chars_a_second;
    private int last_second = -1;
    private float WPM = 0f;
    private float accuracy = 100f;
    private bool timer_active = false;
    public TextMeshProUGUI GUI_typing_text;
    public TextMeshProUGUI GUI_WPM_text;
    public TextMeshProUGUI GUI_accuracy_text;
    public TextMeshProUGUI GUI_trail_sentence_a;
    public TextMeshProUGUI GUI_trail_sentence_b;
    private readonly Array keyCodes = Enum.GetValues(typeof(KeyCode));

    public AudioClip[] typing_sfx;
    private AudioSource audio_source;


    private void GenerateWordList()
    {
        // Reads the word list from the text file and splits it into an array 
        using(StreamReader streamReader = new StreamReader(PlayerData.word_list))
        {
            string text_contents = streamReader.ReadToEnd();
            word_list = text_contents.Split(new char[] {' ', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries);
        }
    }


    private StringBuilder GenerateSentence(StringBuilder sentence)
    {
        // Randomly appends words until it fills the size of the text box
        while(sentence.Length < 27)
        {
            string word_to_add = word_list[UnityEngine.Random.Range(0, word_list.Length)];
            if(sentence.Length + word_to_add.Length <= 31)
            {
                sentence.Append(word_to_add + " ");
            }
        }
        sentence.Append(" ");

        return sentence;
    }


    private void SentenceUpdate()
    {
         typing_sentence = new StringBuilder(trailing_sentence_A.ToString()); 
        trailing_sentence_A = new StringBuilder(trailing_sentence_B.ToString()); 
        trailing_sentence_B = GenerateSentence(new StringBuilder()); 
        cursor_position = 0;
    }


    private void KeyPressValidator()
    {
        // Code for validating key presses from: https://discussions.unity.com/t/find-out-which-key-was-pressed/616242/16

        char cursor_char = typing_sentence[cursor_position];

        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode)) {
                    char char_typed = GetCharacterFromKeyCode(keyCode);
                    total_chars_a_second++;

                    if (!timer_active)
                    {
                        timer_active = true;
                    }

                    if (char_typed == cursor_char)
                    { 
                        cursor_position++;
                        CharTypedCorrect(char_typed);
                        audio_source.PlayOneShot(typing_sfx[0]);
                    }
                    else if(char_typed == '\0')
                    {
                        break;
                    }
                    else
                    {
                        cursor_position++;
                        audio_source.PlayOneShot(typing_sfx[1]);
                        //CharTypedIncorrect()
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
            case KeyCode.Space: return ' ';
            default: return '\0';
        }
    }


    private void CharTypedCorrect(char char_typed)
    {
        correct_chars_a_second++;
        if(char_typed == ' ')
        {
            PlayerData.words_typed_alltime++;
        }
    }


    private void WPMCalculator()
    {
        float minutes_played = timer/60f;
        
        if(correct_chars_list.Count == 101)
        {
            correct_chars_list.RemoveAt(0);
        }

        if(timer > 100.0)
        {
            minutes_played = 1.6f;
        }

        float total_correct_chars = correct_chars_list.Sum();
        WPM = (total_correct_chars/5)/minutes_played;
        WPM = Mathf.Round(WPM * 10f) / 10f;
    }


    private void AccuracyCalculator()
    {
        if(total_chars_list.Count == 101)
        {
            total_chars_list.RemoveAt(0);
        }

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


    private void UpdateTextDisplay()
    {
        // Updates the typing text to move the cursor
        string display_text = typing_sentence.ToString();
        
        display_text = display_text.Insert(cursor_position, "<u>");
        display_text = display_text.Insert(cursor_position + 4, "</u>"); 

        GUI_typing_text.text = display_text;

        //Updates the WPM with the accurate WPM
        GUI_WPM_text.text = $"WPM: {WPM.ToString()}";

        GUI_accuracy_text.text = $"Accuracy: {accuracy.ToString()}%";

        GUI_trail_sentence_a.text = trailing_sentence_A.ToString();
        GUI_trail_sentence_b.text = trailing_sentence_B.ToString();
    }


    void Start()
    {
        GenerateWordList();
        typing_sentence = GenerateSentence(new StringBuilder());
        trailing_sentence_A = GenerateSentence(new StringBuilder());
        trailing_sentence_B = GenerateSentence(new StringBuilder());
        audio_source = GetComponent<AudioSource>();
    }


    void Update()
    {
        UpdateTextDisplay();

        if(cursor_position != typing_sentence.Length- 1)
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
        
        Debug.Log(PlayerData.words_typed_alltime);
    }

    
}
