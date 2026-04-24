using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Hace que el enemigo persiga a un objetivo usando un NavMeshAgent.
/// Actualiza el destino solo cuando el objetivo se mueve lo suficiente
/// para evitar recalcular la ruta innecesariamente en cada frame.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFollowTarget : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Objetivo actual que el enemigo intentara perseguir con el NavMeshAgent.")]
    [SerializeField] private Transform target;
    [Tooltip("Distancia minima que debe moverse el objetivo antes de recalcular la ruta.")]
    [SerializeField] private float repathDistance = 0.5f;

    private NavMeshAgent navMeshAgent;
    private Vector3 lastTargetPosition;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Si no hay objetivo asignado o el agente no está listo, no intentamos moverlo.
        if (target == null || navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        // Solo pedimos una nueva ruta cuando el objetivo realmente cambió de posición.
        if ((target.position - lastTargetPosition).sqrMagnitude < repathDistance * repathDistance)
        {
            return;
        }

        // Guardamos la ultima posicion conocida del objetivo y se la pasamos al agente.
        // Esta llamada es la que hace que el enemigo calcule la ruta y empiece a moverse.
        lastTargetPosition = target.position;
        navMeshAgent.SetDestination(lastTargetPosition);
    }

    /// <summary>
    /// Asigna un nuevo objetivo para que el enemigo lo persiga.
    /// </summary>
    /// <param name="newTarget">Transform del objetivo a seguir.</param>
    public void SetTarget(Transform newTarget)
    {
        if (target == newTarget)
        {
            return;
        }

        target = newTarget;

        if (target != null && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            // Al asignar un objetivo nuevo, forzamos un primer destino inmediato.
            // Luego Update se encarga de recalcular solo cuando el objetivo cambie lo suficiente.
            navMeshAgent.SetDestination(target.position);
            lastTargetPosition = target.position;
        }
    }

    /// <summary>
    /// Limpia el objetivo actual y detiene el movimiento del agente.
    /// </summary>
    public void ClearTarget()
    {
        target = null;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.ResetPath();
        }
    }
}
