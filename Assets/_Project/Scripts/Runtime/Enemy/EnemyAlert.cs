using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyBehaviour))]
/// <summary>
/// Recibe una posicion sospechosa y mueve al enemigo para investigarla mientras mantiene deteccion del jugador.
/// </summary>
public class EnemyAlert : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("NavMeshAgent usado para navegar hacia la posicion sospechosa.")]
    [SerializeField] private NavMeshAgent navAgent;

    [Tooltip("Transform opcional de alerta. Si esta asignado, su posicion se usa como destino actual de investigacion.")]
    [SerializeField] private Transform alertedTransform;

    [Header("Investigacion")]
    [Tooltip("Segundos que el enemigo espera en el punto de alerta antes de volver al flujo normal.")]
    [Min(0f)]
    [SerializeField] private float investigateWaitSeconds = 3f;

    private EnemyBehaviour behaviour;
    private Coroutine investigateCoroutine;
    private Vector3 alertPosition;
    private bool alertDetected;
    private bool isInvestigatingAlert;

    private void Awake()
    {
        // La alerta depende del orquestador para reutilizar la espera con deteccion.
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

        behaviour = GetComponent<EnemyBehaviour>();
    }

    private bool ValidatePrefabSetup()
    {
        bool isValid = true;

        if (navAgent == null)
        {
            Debug.LogError($"[{nameof(EnemyAlert)}] Falta {nameof(NavMeshAgent)} en '{name}'.", this);
            isValid = false;
        }

        if (behaviour == null)
        {
            Debug.LogError($"[{nameof(EnemyAlert)}] Falta {nameof(EnemyBehaviour)} en '{name}'.", this);
            isValid = false;
        }

        // WIP: falta definir que sistema emite alertas y si la fuente canonica sera Transform, evento o posicion directa.
        return isValid;
    }

    public void RaiseAlert(Vector3 position)
    {
        alertPosition = position;
        alertedTransform = null;
        alertDetected = true;
    }

    public void RaiseAlert(Transform alertTransform)
    {
        if (alertTransform == null)
        {
            Debug.LogWarning($"[{nameof(EnemyAlert)}] Se intento activar una alerta sin Transform.", this);
            return;
        }

        alertedTransform = alertTransform;
        alertPosition = alertTransform.position;
        alertDetected = true;
    }

    public void PerformInspection()
    {
        if (!alertDetected || isInvestigatingAlert)
        {
            return;
        }

        Vector3 destination = alertedTransform != null ? alertedTransform.position : alertPosition;
        if (!TrySetDestination(destination))
        {
            ClearAlert();
            return;
        }

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            isInvestigatingAlert = true;
            investigateCoroutine = StartCoroutine(InvestigateAlert());
        }
    }

    public bool HasAlert()
    {
        return alertDetected || isInvestigatingAlert;
    }

    public void ClearAlert()
    {
        if (investigateCoroutine != null)
        {
            StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
        }

        alertDetected = false;
        isInvestigatingAlert = false;
    }

    private IEnumerator InvestigateAlert()
    {
        navAgent.isStopped = true;

        yield return StartCoroutine(behaviour.WaitWithDetection(investigateWaitSeconds));

        alertDetected = false;
        isInvestigatingAlert = false;
        investigateCoroutine = null;
        navAgent.isStopped = false;
        navAgent.ResetPath();
    }

    private bool TrySetDestination(Vector3 destination)
    {
        if (navAgent == null || !navAgent.isActiveAndEnabled || !navAgent.isOnNavMesh)
        {
            Debug.LogWarning($"[{nameof(EnemyAlert)}] No se pudo investigar la alerta porque el {nameof(NavMeshAgent)} no esta listo.", this);
            return false;
        }

        return navAgent.SetDestination(destination);
    }
}
