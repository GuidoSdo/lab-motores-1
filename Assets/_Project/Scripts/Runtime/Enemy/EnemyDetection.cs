using UnityEngine;

/// <summary>
/// Detecta al jugador dentro del radio y cono de vision del enemigo, validando capas y linea de vision.
/// </summary>
public class EnemyDetection : MonoBehaviour
{
    [Header("Deteccion")]
    [Tooltip("Radio maximo usado por OverlapSphere para buscar posibles objetivos.")]
    [SerializeField] private float detectionRadius = 8f;
    [Tooltip("Angulo total del cono de vision. El objetivo debe quedar dentro de la mitad izquierda o derecha de este valor.")]
    [SerializeField] private float fieldOfView = 120f;
    [Tooltip("Capas que contienen colliders detectables, normalmente el jugador y sus hijos.")]
    [SerializeField] private LayerMask targetMask;
    [Tooltip("Capas que bloquean el raycast de vision entre el enemigo y el objetivo.")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Depuracion")]
    [Tooltip("Dibuja en Scene View el ultimo raycast usado para comprobar linea de vision.")]
    [SerializeField] private bool drawDetectionRay = true;
    [Tooltip("Muestra logs de diagnostico sobre candidatos detectados, filtrados u obstruidos.")]
    [SerializeField] private bool enableDebugLogs = false;
    [Tooltip("Dibuja lineas temporales en Play Mode para visualizar raycasts de deteccion.")]
    [SerializeField] private bool drawRuntimeDebugLines = true;

    private Vector3 lastRayOrigin;
    private Vector3 lastRayDirection;
    private float lastRayDistance;
    private bool hasRayToDraw;
    private bool lastRayWasBlocked;
   

    public Transform DetectTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);

        LogDebug($"Se detectaron {hits.Length} collider(s) dentro del radio.");

        foreach (Collider hit in hits)
        {
            // El collider detectado puede estar en un hijo del jugador; la identidad real se valida en la jerarquia padre.
            PlayerController playerController = hit.GetComponentInParent<PlayerController>();
            if (playerController == null)
            {
                LogDebug($"El collider '{hit.name}' entro en el targetMask, pero no pertenece a un PlayerController.");
                continue;
            }

            if (playerController.IsHidden)
            {
                LogDebug($"Se ignora a '{playerController.name}' porque esta escondido.");
                continue;
            }

            Transform candidate = playerController.transform;
            Vector3 directionToTarget = (candidate.position - transform.position).normalized;

            float angle = Vector3.Angle(transform.forward, directionToTarget);
            if (angle > fieldOfView * 0.5f)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, candidate.position);
            SaveDebugRay(transform.position, directionToTarget, distance);

            if (!Physics.Raycast(transform.position, directionToTarget, distance, obstacleMask))
            {
                lastRayWasBlocked = false;
                DrawRuntimeDebugLine(transform.position, directionToTarget, distance, Color.green);
                LogDebug($"Objetivo detectado: '{candidate.name}'.");
                return candidate;
            }

            // WIP: falta definir si las puertas bloquean vision siempre o dependen de su estado abierto/cerrado.
            lastRayWasBlocked = true;
            DrawRuntimeDebugLine(transform.position, directionToTarget, distance, Color.yellow);
            LogDebug($"'{candidate.name}' fue detectado, pero hay un obstaculo bloqueando la vision.");
        }

        hasRayToDraw = false;
        LogDebug("No se detecto ningun objetivo valido.");
        return null;
    }
    

   
    private void SaveDebugRay(Vector3 origin, Vector3 direction, float distance)
    {
        lastRayOrigin = origin;
        lastRayDirection = direction;
        lastRayDistance = distance;
        hasRayToDraw = true;
    }

    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[EnemyDetection] {message}", this);
    }

    private void DrawRuntimeDebugLine(Vector3 origin, Vector3 direction, float distance, Color color)
    {
        if (!drawRuntimeDebugLines)
        {
            return;
        }

        Debug.DrawLine(origin, origin + direction * distance, color);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Los gizmos muestran el cono configurado aunque no haya un objetivo detectado en runtime.
        Vector3 leftBoundary = Quaternion.Euler(0f, -fieldOfView * 0.5f, 0f) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0f, fieldOfView * 0.5f, 0f) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);

        if (drawDetectionRay && hasRayToDraw)
        {
            Gizmos.color = lastRayWasBlocked ? Color.yellow : Color.green;
            Gizmos.DrawLine(lastRayOrigin, lastRayOrigin + lastRayDirection * lastRayDistance);
        }
    }
}

