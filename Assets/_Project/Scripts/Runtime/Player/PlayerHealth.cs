using UnityEngine;

/// <summary>
/// Gestiona la muerte del jugador. El contacto con el enemigo es instakill.
/// </summary>
[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Scene Routing")]
    [Tooltip("Nombre o path exacto de la escena de derrota en Build Settings.")]
    [SerializeField] private string _defeatSceneName = "Assets/_Project/Scenes/SC_Defeat.unity";

    private bool _isDead;

    public bool IsDead => _isDead;

    private void Awake()
    {
        ResetDeathState();
    }

    public void ResetDeathState()
    {
        _isDead = false;
    }

    public void Kill()
    {
        Die();
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        Debug.Log("<color=red>[PLAYER HEALTH]</color> Instakill detectado. Cargando escena de derrota.", this);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneLoader.TryLoadScene(_defeatSceneName, this);
    }
}
