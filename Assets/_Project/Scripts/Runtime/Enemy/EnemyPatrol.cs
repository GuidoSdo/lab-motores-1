using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NavMeshAgent navAgent;


    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolSets;
    [SerializeField] private int pointIndex;
    [SerializeField] private int setIndex;
    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;

    private EnemyBehaviour behaviour;
    bool isWaiting = false;


    private void Start()
    {
        pointIndex = 0;
        setIndex = 0;
    }
    private void Awake()
    {
        behaviour = GetComponent<EnemyBehaviour>();
    }



    private void FindPatrolPoint()
    {
        Transform actualSet = patrolSets[setIndex];
        Transform nextPoint = actualSet.GetChild(pointIndex);


        Vector3 nextPatrolPoint = nextPoint.position;

        if (Physics.Raycast(nextPatrolPoint, -transform.up, 2f, groundLayer))
        {
            currentPatrolPoint = nextPatrolPoint;
            hasPatrolPoint = true;
            pointIndex++;
            if (pointIndex >= actualSet.childCount)
            {
                pointIndex = 0;
                setIndex++;
                
            }
        }
        if (setIndex >= patrolSets.Length)
        {
            setIndex = 0;
        }


    }

    public void  PerformPatrol()
    {
        if (isWaiting) return;
        if (!hasPatrolPoint) FindPatrolPoint();


        if (hasPatrolPoint) navAgent.SetDestination(currentPatrolPoint);



        if (hasPatrolPoint && Vector3.Distance(transform.position, currentPatrolPoint) < 1f) 
        {
            
            StartCoroutine(WaitAtPatrolPoint());
        } 
    }
    

    IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        navAgent.isStopped = true;

        yield return StartCoroutine(behaviour.WaitWithDetection(3));

        navAgent.isStopped = false;
        hasPatrolPoint = false;
        isWaiting = false;
        navAgent.ResetPath();
    }

}
