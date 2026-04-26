using UnityEngine;

/// <summary>
/// Gestiona la vida del jugador y centraliza el ingreso de dano y curacion.
/// </summary>
[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Cantidad maxima de vida que puede tener el jugador.")]
    [Min(1)]
    [SerializeField] private int _maxHealth = 100;

    private int _currentHealth;
    private bool _isDead;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
        {
            return;
        }

        _currentHealth = Mathf.Max(0, _currentHealth - damage);

        if (_currentHealth == 0)
        {
            Die();
        }
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
        Debug.Log("El jugador murio.", this);
    }
}
