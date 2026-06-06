using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyAlert))]
[RequireComponent(typeof(EnemyChase))]
/// <summary>
/// Orquesta el estado activo del enemigo y delega movimiento, deteccion, patrulla y alertas en componentes especializados.
/// </summary>
public class EnemyBehaviour : MonoBehaviour
{
    private enum EnemyState
    {
        None,
        Patrol,
        Chase,
        Search,
        Alert
    }

    [Header("Referencias")]
    [Tooltip("NavMeshAgent compartido por persecucion, busqueda, patrulla e investigacion de alertas.")]
    [SerializeField] private NavMeshAgent navAgent;

    [Header("Busqueda")]
    [Tooltip("Segundos que el enemigo permanece atento en la ultima posicion conocida del jugador antes de volver a patrullar.")]
    [Min(0f)]
    [SerializeField] private float searchWaitSeconds = 3f;

    [Header("Depuracion")]
    [Tooltip("Activa logs cuando el enemigo entra o sale de patrulla, persecucion, busqueda o alerta.")]
    [SerializeField] private bool enableStateDebugLogs = true;

    private EnemyDetection detection;
    private EnemyPatrol patrol;
    private EnemyAlert alert;
    private EnemyChase chase;
    private Coroutine searchCoroutine;
    private Vector3 lastKnownPlayerPosition;
    private EnemyState currentState = EnemyState.None;
    private bool wasChasingPlayer;
    private bool isSearching;
    private bool isInitialized;

    private void Awake()
    {
        // El orquestador valida dependencias antes de que Update empiece a cambiar estados.
        CacheReferences();
        isInitialized = ValidatePrefabSetup();
    }

    private void OnValidate()
    {
        CacheReferences();
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        UpdateBehaviourState();
    }

    private void OnDisable()
    {
        if (currentState != EnemyState.None)
        {
            LogStateExit(currentState);
            currentState = EnemyState.None;
        }
    }

    private void CacheReferences()
    {
        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        detection = GetComponent<EnemyDetection>();
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();
        alert = GetComponent<EnemyAlert>();
    }

    private bool ValidatePrefabSetup()
    {
        bool isValid = true;

        isValid &= ValidateReference(navAgent, nameof(navAgent));
        isValid &= ValidateReference(detection, nameof(EnemyDetection));
        isValid &= ValidateReference(patrol, nameof(EnemyPatrol));
        isValid &= ValidateReference(chase, nameof(EnemyChase));
        isValid &= ValidateReference(alert, nameof(EnemyAlert));

        EnemyAlert[] alertComponents = GetComponents<EnemyAlert>();
        if (alertComponents.Length != 1)
        {
            Debug.LogError(
                $"[{nameof(EnemyBehaviour)}] '{name}' debe tener exactamente un {nameof(EnemyAlert)}. Componentes encontrados: {alertComponents.Length}.",
                this);
            isValid = false;
        }

        if (navAgent != null && !navAgent.isOnNavMesh)
        {
            Debug.LogWarning(
                $"[{nameof(EnemyBehaviour)}] '{name}' tiene {nameof(NavMeshAgent)}, pero todavia no esta sobre NavMesh. El movimiento se validara antes de pedir destinos.",
                this);
        }

        enabled = isValid;
        return isValid;
    }

    private bool ValidateReference(Object reference, string referenceName)
    {
        if (reference != null)
        {
            return true;
        }

        Debug.LogError($"[{nameof(EnemyBehaviour)}] Falta referencia requerida: {referenceName} en '{name}'.", this);
        return false;
    }

    private void UpdateBehaviourState()
    {
        Transform detectedPlayer = detection.DetectTarget();

        if (detectedPlayer != null)
        {
            CancelSearch();
            SetState(EnemyState.Chase);
            wasChasingPlayer = true;
            lastKnownPlayerPosition = detectedPlayer.position;
            alert.ClearAlert();
            chase.PerformChase(detectedPlayer);
            return;
        }

        if (isSearching)
        {
            return;
        }

        if (wasChasingPlayer)
        {
            wasChasingPlayer = false;
            searchCoroutine = StartCoroutine(SearchForPlayer(lastKnownPlayerPosition));
            return;
        }

        if (alert.HasAlert())
        {
            SetState(EnemyState.Alert);
            alert.PerformInspection();
            return;
        }

        SetState(EnemyState.Patrol);
        patrol.PerformPatrol();
    }

    private void CancelSearch()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }

        isSearching = false;
        navAgent.isStopped = false;
    }

    public IEnumerator SearchForPlayer(Vector3 targetPosition)
    {
        isSearching = true;
        SetState(EnemyState.Search);

        if (!TrySetDestination(targetPosition))
        {
            isSearching = false;
            searchCoroutine = null;
            yield break;
        }

        while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            if (detection.DetectTarget() != null)
            {
                isSearching = false;
                searchCoroutine = null;
                yield break;
            }

            yield return null;
        }

        yield return StartCoroutine(WaitWithDetection(searchWaitSeconds));

        isSearching = false;
        searchCoroutine = null;
        navAgent.isStopped = false;
        navAgent.ResetPath();
    }

    public IEnumerator WaitWithDetection(float duration)
    {
        // Esta espera se comparte entre busqueda, patrulla y alerta para que cualquier estado pueda interrumpirse al ver al jugador.
        float timer = 0f;

        while (timer < duration)
        {
            if (detection.DetectTarget() != null)
            {
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Cancela la persecucion y busqueda actuales para volver al flujo de patrulla.
    /// </summary>
    public bool AbortPursuit()
    {
        bool isPursuingPlayer =
            currentState == EnemyState.Chase ||
            currentState == EnemyState.Search ||
            wasChasingPlayer ||
            isSearching;

        if (!isPursuingPlayer)
        {
            return false;
        }

        CancelSearch();
        wasChasingPlayer = false;

        if (navAgent != null && navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
            navAgent.velocity = Vector3.zero;
        }

        SetState(EnemyState.Patrol);
        return true;
    }

    private bool TrySetDestination(Vector3 destination)
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled || !navAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[{nameof(EnemyBehaviour)}] No se pudo navegar porque el {nameof(NavMeshAgent)} no esta listo.", this);
            return false;
        }

        return navAgent.SetDestination(destination);
    }

    private void SetState(EnemyState nextState)
    {
        if (currentState == nextState)
        {
            return;
        }

        if (currentState != EnemyState.None)
        {
            LogStateExit(currentState);
        }

        currentState = nextState;
        LogStateEnter(currentState);
    }

    private void LogStateEnter(EnemyState state)
    {
        if (enableStateDebugLogs)
        {
            Debug.Log($"[{nameof(EnemyBehaviour)}] '{name}' entra en estado {state}.", this);
        }
    }

    private void LogStateExit(EnemyState state)
    {
        if (enableStateDebugLogs)
        {
            Debug.Log($"[{nameof(EnemyBehaviour)}] '{name}' sale de estado {state}.", this);
        }
    }
}
