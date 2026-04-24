using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Orquesta los componentes principales del prefab del enemigo.
/// Este controlador no navega directamente, pero exige un NavMeshAgent
/// porque delega el movimiento al componente EnemyFollowTarget, que sí lo necesita.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyFollowTarget))]
[RequireComponent(typeof(EnemyDetection))]
public class EnemyController : MonoBehaviour
{
    private EnemyDetection detection;
    private EnemyFollowTarget followTarget;

    private void Awake()
    {
        detection = GetComponent<EnemyDetection>();
        followTarget = GetComponent<EnemyFollowTarget>();
    }

    private void Update()
    {
        Transform detectedTarget = detection.DetectTarget();

        // Si detectamos un objetivo, le indicamos al componente de seguimiento
        // que lo persiga. Si no hay objetivo visible, detenemos la persecucion.
        if (detectedTarget != null)
        {
            followTarget.SetTarget(detectedTarget);
        }
        else
        {
            followTarget.ClearTarget();
        }
    }
}
