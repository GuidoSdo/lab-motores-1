using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga una escena desde un trigger y libera el cursor para pantallas de UI.
/// </summary>
public class SceneSwapTrigger : MonoBehaviour
{
    [Tooltip("Nombre de la escena que se carga al entrar en este trigger.")]
    [SerializeField] private string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(sceneName);
    }
}
