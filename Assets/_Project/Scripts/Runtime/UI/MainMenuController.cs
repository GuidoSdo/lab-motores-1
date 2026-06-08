using UnityEngine;

/// <summary>
/// Atiende las acciones principales del menu inicial.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Nombre de la escena que se carga al iniciar la partida.")]
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
