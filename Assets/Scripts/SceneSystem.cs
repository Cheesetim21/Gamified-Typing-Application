using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private int scene_code;

    private void SwitchScene(int code)
    {
        SceneManager.LoadScene(code);
    }
    

    void Start()
    {
        scene_code = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) && scene_code != 5)
        {
            scene_code = scene_code + 1;
            SwitchScene(scene_code);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && scene_code != 0)
        {
            scene_code = scene_code - 1;
            SwitchScene(scene_code);
        }
    }
}
