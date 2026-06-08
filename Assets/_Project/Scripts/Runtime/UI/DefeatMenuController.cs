using UnityEngine;

/// <summary>
/// Gestiona las acciones de navegacion disponibles desde la escena de derrota.
/// </summary>
public class DefeatMenuController : MonoBehaviour
{
    [Header("Scenes")]
    [Tooltip("Nombre exacto de la escena que se carga al reintentar.")]
    [SerializeField] private string gameSceneName = "SC_Gameplay_L01";
    [Tooltip("Nombre exacto de la escena del menu principal.")]
    [SerializeField] private string mainMenuSceneName = "SC_MainMenu";

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
