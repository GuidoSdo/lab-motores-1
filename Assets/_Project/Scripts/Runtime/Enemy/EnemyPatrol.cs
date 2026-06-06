using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyBehaviour))]
/// <summary>
/// Recorre puntos hijos de GameObjects padre asignados en Patrol Sets desde el Inspector.
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("NavMeshAgent que recibe los destinos de patrulla.")]
    [SerializeField] private NavMeshAgent navAgent;

    [Header("Capas")]
    [Tooltip("Capa usada por el raycast vertical para validar que un punto de patrulla tenga suelo debajo.")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Patrulla")]
    [Tooltip("Arrastrar aqui el GameObject padre de cada ruta. Sus hijos se usan como puntos de patrulla en orden, de arriba hacia abajo en la jerarquia.")]
    [SerializeField] private Transform[] patrolSets;

    [Tooltip("Distancia maxima al punto actual para considerarlo alcanzado y comenzar la espera.")]
    [Min(0.05f)]
    [SerializeField] private float arrivalDistance = 1f;

    [Tooltip("Segundos que el enemigo permanece detenido en cada punto de patrulla mientras sigue detectando al jugador.")]
    [Min(0f)]
    [SerializeField] private float waitAtPointSeconds = 3f;

    private EnemyBehaviour behaviour;
    private Vector3 currentPatrolPoint;
    private Coroutine waitCoroutine;
    private int pointIndex;
    private int setIndex;
    private bool hasPatrolPoint;
    private bool isWaiting;
    private bool isValidSetup;

    /*
     * Uso en escena:
     * 1. Crear un Empty por ruta, por ejemplo EnemyPatrolSet_A.
     * 2. Crear hijos dentro de ese Empty: PatrolPoint_01, PatrolPoint_02, PatrolPoint_03.
     * 3. Ubicar cada hijo sobre suelo navegable y con collider/layer compatible con Ground Layer.
     * 4. Arrastrar el Empty padre al array Patrol Sets del EnemyPatrol.
     * El script recorre los hijos en el orden de la jerarquia y luego continua con el siguiente set.
     */

    private void Awake()
    {
        // La validacion evita que una patrulla mal configurada rompa el Update del orquestador.
        CacheReferences();
        isValidSetup = ValidatePrefabSetup();
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

        behaviour = GetComponent<EnemyBehaviour>();
    }

    private bool ValidatePrefabSetup()
    {
        bool isValid = true;

        if (navAgent == null)
        {
            Debug.LogError($"[{nameof(EnemyPatrol)}] Falta {nameof(NavMeshAgent)} en '{name}'.", this);
            isValid = false;
        }

        if (behaviour == null)
        {
            Debug.LogError($"[{nameof(EnemyPatrol)}] Falta {nameof(EnemyBehaviour)} en '{name}'.", this);
            isValid = false;
        }

        if (patrolSets == null || patrolSets.Length == 0)
        {
            Debug.LogError($"[{nameof(EnemyPatrol)}] '{name}' no tiene sets de patrulla asignados.", this);
            isValid = false;
        }
        else
        {
            for (int i = 0; i < patrolSets.Length; i++)
            {
                if (patrolSets[i] == null)
                {
                    Debug.LogError($"[{nameof(EnemyPatrol)}] El set de patrulla {i} esta sin asignar en '{name}'.", this);
                    isValid = false;
                    continue;
                }

                if (patrolSets[i].childCount == 0)
                {
                    Debug.LogError($"[{nameof(EnemyPatrol)}] El set de patrulla '{patrolSets[i].name}' no tiene puntos hijos.", this);
                    isValid = false;
                }
            }
        }

        // WIP: falta definir si un enemigo sin patrulla valida debe quedar idle o si siempre debe fallar la configuracion.
        return isValid;
    }

    public void PerformPatrol()
    {
        if (!isValidSetup || isWaiting)
        {
            return;
        }

        if (!hasPatrolPoint && !TryFindPatrolPoint())
        {
            return;
        }

        if (!TrySetDestination(currentPatrolPoint))
        {
            return;
        }

        if (Vector3.Distance(transform.position, currentPatrolPoint) <= arrivalDistance)
        {
            waitCoroutine = StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private bool TryFindPatrolPoint()
    {
        if (patrolSets == null || patrolSets.Length == 0)
        {
            return false;
        }

        int maxAttempts = CountConfiguredPatrolPoints();
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Transform currentSet = patrolSets[setIndex];
            if (currentSet != null && currentSet.childCount > 0)
            {
                Transform nextPoint = currentSet.GetChild(pointIndex);
                AdvanceIndexes(currentSet.childCount);

                if (Physics.Raycast(nextPoint.position, Vector3.down, 2f, groundLayer))
                {
                    currentPatrolPoint = nextPoint.position;
                    hasPatrolPoint = true;
                    return true;
                }

                Debug.LogWarning(
                    $"[{nameof(EnemyPatrol)}] El punto '{nextPoint.name}' no tiene suelo valido debajo. Se omite.",
                    this);
            }
            else
            {
                AdvanceToNextSet();
            }
        }

        Debug.LogWarning($"[{nameof(EnemyPatrol)}] No se encontro ningun punto de patrulla valido para '{name}'.", this);
        return false;
    }

    private int CountConfiguredPatrolPoints()
    {
        int count = 0;

        for (int i = 0; i < patrolSets.Length; i++)
        {
            if (patrolSets[i] != null)
            {
                count += patrolSets[i].childCount;
            }
        }

        return Mathf.Max(1, count);
    }

    private void AdvanceIndexes(int pointCount)
    {
        pointIndex++;
        if (pointIndex < pointCount)
        {
            return;
        }

        AdvanceToNextSet();
    }

    private void AdvanceToNextSet()
    {
        pointIndex = 0;
        setIndex++;

        if (setIndex >= patrolSets.Length)
        {
            setIndex = 0;
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        navAgent.isStopped = true;

        yield return StartCoroutine(behaviour.WaitWithDetection(waitAtPointSeconds));

        navAgent.isStopped = false;
        hasPatrolPoint = false;
        isWaiting = false;
        waitCoroutine = null;
        navAgent.ResetPath();
    }

    private bool TrySetDestination(Vector3 destination)
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled || !navAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[{nameof(EnemyPatrol)}] No se pudo patrullar porque el {nameof(NavMeshAgent)} no esta listo.", this);
            return false;
        }

        return navAgent.SetDestination(destination);
    }
}
