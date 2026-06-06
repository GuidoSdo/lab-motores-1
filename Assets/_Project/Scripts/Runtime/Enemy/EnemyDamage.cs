using UnityEngine;

/// <summary>
/// Aplica dano al PlayerHealth del jugador cuando entra en el trigger de contacto del enemigo.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class EnemyDamage : MonoBehaviour
{
    [Header("Damage")]
    [Tooltip("Cantidad de vida que se descuenta al jugador cuando entra en el trigger del enemigo.")]
    [Min(1)]
    [SerializeField] private int damageToPlayer = 10;

    [Header("Depuracion")]
    [Tooltip("Activa logs para diagnosticar contactos ignorados y dano aplicado.")]
    [SerializeField] private bool enableDebugLogs;

    private Collider damageCollider;

    private void Awake()
    {
        // El collider debe existir antes de que Unity dispare eventos de trigger.
        damageCollider = GetComponent<Collider>();
        ValidatePrefabSetup();
    }

    private void ValidatePrefabSetup()
    {
        if (damageCollider == null)
        {
            Debug.LogError($"[{nameof(EnemyDamage)}] Falta Collider en '{name}'.", this);
            enabled = false;
            return;
        }

        if (!damageCollider.isTrigger)
        {
            Debug.LogWarning(
                $"[{nameof(EnemyDamage)}] El Collider de '{name}' deberia tener Is Trigger activado para aplicar dano por contacto.",
                this);
        }

        // WIP: falta definir si el dano debe tener cooldown, knockback o aplicarse una sola vez por contacto.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            LogDebug($"Se ignora '{other.name}' porque no tiene tag Player.");
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogWarning(
                $"[{nameof(EnemyDamage)}] '{other.name}' tiene tag Player, pero no se encontro {nameof(PlayerHealth)} en su jerarquia.",
                this);
            return;
        }

        playerHealth.TakeDamage(damageToPlayer);
        LogDebug($"Se aplico {damageToPlayer} de dano a '{playerHealth.name}'.");
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{nameof(EnemyDamage)}] {message}", this);
        }
    }
}
