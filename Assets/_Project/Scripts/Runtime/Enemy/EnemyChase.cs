using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour

{
    [SerializeField] private NavMeshAgent navAgent;
    public void PerformChase(Transform transform)
    {
        
        
        navAgent.SetDestination(transform.position);
        

    }
}
