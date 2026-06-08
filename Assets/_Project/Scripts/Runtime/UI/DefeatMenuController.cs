using UnityEngine;

public class DefeatMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "SC_Gameplay_L01";
    [SerializeField] private string mainMenuSceneName = "SC_MainMenu";

    public void OnRetryButtonPressed()
    {
        SceneLoader.TryLoadScene(gameSceneName, this);
    }

    public void OnMainMenuButtonPressed()
    {
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
