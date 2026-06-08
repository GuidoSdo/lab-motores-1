using UnityEngine;

/// <summary>
/// Mata al jugador cuando entra en el trigger de contacto del enemigo.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class EnemyDamage : MonoBehaviour
{
    [Header("Depuracion")]
    [Tooltip("Activa logs para diagnosticar contactos ignorados e instakill aplicado.")]
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

        playerHealth.Kill();
        LogDebug($"Se aplico instakill a '{playerHealth.name}'.");
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{nameof(EnemyDamage)}] {message}", this);
        }
    }
}
