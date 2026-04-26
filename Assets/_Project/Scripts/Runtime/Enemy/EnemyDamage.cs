using UnityEngine;

/// <summary>
/// Aplica dano al jugador cuando entra en el trigger del enemigo.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class EnemyDamage : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("Cantidad de vida que el enemigo le quita al jugador por contacto.")]
    [Min(1)]
    [SerializeField] private int _damageToPlayer = 10;

    [Header("Debug")]
    [Tooltip("Activa logs de depuracion para entender como el enemigo detecta y aplica dano.")]
    [SerializeField] private bool _enableDebugLogs = true;

    private Collider _damageCollider;

    private void Awake()
    {
        _damageCollider = GetComponent<Collider>();

        if (!_damageCollider.isTrigger)
        {
            Debug.LogWarning(
                $"[{nameof(EnemyDamage)}] El collider en '{name}' deberia tener 'Is Trigger' activado para aplicar dano por contacto.",
                this);
        }

        LogDebug(
            $"[{nameof(EnemyDamage)}] Inicializado en '{name}'. Requiere un Collider con 'Is Trigger' y un objetivo con {nameof(PlayerHealth)} en su jerarquia. Dano configurado: {_damageToPlayer}.");
    }

    private void OnTriggerEnter(Collider other)
    {
        LogDebug($"[{nameof(EnemyDamage)}] Trigger detectado con '{other.name}' (tag: '{other.tag}').");

        if (!other.CompareTag("Player"))
        {
            LogDebug($"[{nameof(EnemyDamage)}] Se ignora '{other.name}' porque no tiene el tag 'Player'.");
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning(
                $"[{nameof(EnemyDamage)}] '{other.name}' tiene tag 'Player', pero no se encontro {nameof(PlayerHealth)} en su jerarquia.",
                this);
            return;
        }

        LogDebug(
            $"[{nameof(EnemyDamage)}] Aplicando {_damageToPlayer} de dano a '{playerHealth.name}'. Vida actual antes del impacto: {playerHealth.CurrentHealth}.");
        playerHealth.TakeDamage(_damageToPlayer);
        LogDebug($"[{nameof(EnemyDamage)}] Dano aplicado a '{playerHealth.name}'. Vida restante: {playerHealth.CurrentHealth}.");
    }

    private void LogDebug(string message)
    {
        if (!_enableDebugLogs)
        {
            return;
        }

        Debug.Log(message, this);
    }
}
