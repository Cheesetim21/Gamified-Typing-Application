using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class TypingHelpButton : MonoBehaviour
{
    public GameObject typing_tutorial;
    private bool tutorial_shown = false;

    public void PressTutorialButton()
    {
        tutorial_shown = true;
        typing_tutorial.SetActive(true);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        typing_tutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(tutorial_shown)
        {
            if(Input.anyKeyDown)
            {
                tutorial_shown = false;
                typing_tutorial.SetActive(false);
            }
        }
    }
}
