using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

/// <summary>
/// Se encarga de detectar si el jugador esta dentro del rango de vision del enemigo.
/// Usa capas para filtrar candidatos rapidamente y PlayerController para validar
/// que el objetivo detectado realmente pertenezca al jugador.
/// </summary>
public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Radio maximo dentro del cual el enemigo busca posibles objetivos.")]
    [SerializeField] private float detectionRadius = 8f;
    [Tooltip("Angulo total de vision del enemigo para validar si puede ver al objetivo.")]
    [SerializeField] private float fieldOfView = 120f;
    [Tooltip("Capas que contienen a los posibles objetivos detectables, como el jugador.")]
    [SerializeField] private LayerMask targetMask;
    [Tooltip("Capas que bloquean la linea de vision entre el enemigo y el objetivo.")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Debug")]
    [Tooltip("Dibuja en la escena el ultimo raycast usado para comprobar la linea de vision.")]
    [SerializeField] private bool drawDetectionRay = true;
    [Tooltip("Muestra mensajes en consola para entender por que un objetivo fue o no detectado.")]
    [SerializeField] private bool enableDebugLogs = false;
    [Tooltip("Dibuja lineas de depuracion en tiempo de ejecucion para visualizar la deteccion.")]
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
            // El collider detectado puede pertenecer a un hijo del prefab del jugador.
            // Por eso validamos el PlayerController en la jerarquia padre.
            PlayerController playerController = hit.GetComponentInParent<PlayerController>();
            if (playerController == null)
            {
                LogDebug($"El collider '{hit.name}' entro en el targetMask, pero no pertenece a un PlayerController.");
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

            // TODO: Aca deberiamos considerar codear los obstaculos de las puertas?
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

        // Dibujamos los limites del campo de vision para visualizar el cono de deteccion.
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

