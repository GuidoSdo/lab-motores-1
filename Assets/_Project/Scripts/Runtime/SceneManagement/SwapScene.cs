using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cambia de escena cuando el jugador entra en el trigger asociado.
/// </summary>
public class SwapScene : MonoBehaviour
{
    [Tooltip("Nombre de la escena que se carga al activar el trigger.")]
    [SerializeField] private string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
