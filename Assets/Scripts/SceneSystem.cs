using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private static SceneSystem instance;
    private int scene_index;

    private bool scene_1_locked = true;
    private bool scene_2_locked = true;

    private void UpdateUnlockStatus()
    {
        scene_1_locked = PlayerData.upgrade_dict["statistics"] != 1;
        scene_2_locked = PlayerData.upgrade_dict["rankings"] != 1;
    }

    private bool IsSceneLocked(int scene_index)
    {
        return (scene_index == 1 && scene_1_locked) || (scene_index == 2 && scene_2_locked);
    }

    public void SwitchSceneRight()
    {
        UpdateUnlockStatus();

        do
        {
            scene_index++;
        } while (IsSceneLocked(scene_index) && scene_index <= 5);

        SceneManager.LoadScene(scene_index);
    }

    public void SwitchSceneLeft()
    {
        UpdateUnlockStatus();

        do
        {
            scene_index--;
        } while (IsSceneLocked(scene_index) && scene_index >= 0);

        SceneManager.LoadScene(scene_index);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            scene_index = SceneManager.GetActiveScene().buildIndex;

            if (scene_index != 4)
            {
                SceneManager.LoadScene(4);
                scene_index = 4;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) && scene_index != 5)
        {
            
            SwitchSceneRight();
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && scene_index != 0)
        {
            SwitchSceneLeft();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            QuitGame();
        }
    }
}
