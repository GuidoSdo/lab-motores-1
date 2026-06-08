using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Sincroniza los parametros del Animator del enemigo con su velocidad de navegacion.
/// </summary>
public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
