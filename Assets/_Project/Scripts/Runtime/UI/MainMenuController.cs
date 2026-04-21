using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "SC_Gameplay_L01";

    public void OnPlayButtonPressed()
    {
        SceneLoader.TryLoadScene(gameSceneName, this);
    }

    public void OnOptionsButtonPressed()
    {
        Debug.Log("Options menu is not implemented yet.");
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
