using UnityEngine;

/// <summary>
/// Gestiona la vida del jugador. Diseñado para un sistema Instakill (un golpe es mortal).
/// Al morir, redirige automáticamente a la escena de derrota usando el sistema del proyecto.
/// </summary>
[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Cantidad maxima de vida que puede tener el jugador.")]
    [Min(1)]
    [SerializeField] private int _maxHealth = 100;

    [Header("Scene Routing")]
    [Tooltip("Nombre exacto de la escena de derrota en el proyecto.")]
    [SerializeField] private string _defeatSceneName = "DefeatScene"; // <-- Poné acá el nombre real de tu escena

    private int _currentHealth;
    private bool _isDead;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        ResetFullStats();
    }

    public void ResetFullStats()
    {
        _isDead = false;
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
        {
            return;
        }

        _currentHealth = 0;
        Die();
    }

    public void Heal(int amount)
    {
        if (_isDead || amount <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        Debug.Log("<color=red>[PLAYER HEALTH]</color> Muerte por Instakill detectada. Viajando a la escena de derrota...");

        // 🐭 Liberamos el mouse antes de irnos para que el jugador pueda clickear los botones del menú de derrota
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🎬 PLAN DIRECTO: Cargamos la escena de derrota usando tu SceneLoader nativo
        SceneLoader.TryLoadScene(_defeatSceneName, this);
    }
}