using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Actualiza el destino del NavMeshAgent para perseguir al objetivo visible sin recalcular rutas innecesarias.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChase : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("NavMeshAgent que recibe los destinos de persecucion.")]
    [SerializeField] private NavMeshAgent navAgent;

    [Header("Persecucion")]
    [Tooltip("Distancia minima que debe moverse el objetivo para pedir una nueva ruta al NavMeshAgent.")]
    [Min(0f)]
    [SerializeField] private float repathDistance = 0.25f;

    private Vector3 lastDestination;
    private bool hasDestination;

    private void Awake()
    {
        // La referencia se cachea tambien en Awake porque OnValidate no corre en builds.
        CacheReferences();
        ValidatePrefabSetup();
    }

    private void OnValidate()
    {
        CacheReferences();
    }

    private void CacheReferences()
    {
        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }
    }

    private bool ValidatePrefabSetup()
    {
        if (navAgent != null)
        {
            return true;
        }

        Debug.LogError($"[{nameof(EnemyChase)}] Falta {nameof(NavMeshAgent)} en '{name}'.", this);
        enabled = false;
        return false;
    }

    public void PerformChase(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (navAgent == null || !navAgent.isActiveAndEnabled || !navAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[{nameof(EnemyChase)}] No se pudo perseguir porque el {nameof(NavMeshAgent)} no esta listo.", this);
            return;
        }

        if (hasDestination && (target.position - lastDestination).sqrMagnitude < repathDistance * repathDistance)
        {
            return;
        }

        lastDestination = target.position;
        hasDestination = true;
        navAgent.isStopped = false;
        navAgent.SetDestination(lastDestination);
    }
}
