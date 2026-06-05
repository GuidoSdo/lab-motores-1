using UnityEngine;

public class DefeatMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "SC_Gameplay_L01";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void OnRetryButtonPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.TryLoadScene(gameSceneName, this);
    }

    public void OnMainMenuButtonPressed()
    {
        Time.timeScale = 1f;
        SceneLoader.TryLoadScene(mainMenuSceneName, this);
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}