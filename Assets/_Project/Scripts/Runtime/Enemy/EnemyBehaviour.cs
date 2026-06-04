using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyAlert))]
[RequireComponent(typeof(EnemyChase))]
public class EnemyBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private Transform playerTransform;
    Vector3 lastKnownPlayerPosition;
    private bool wasChasingPlayer;
    private bool isSearching;
    private EnemyDetection detection;
    private EnemyPatrol patrol;
    private EnemyAlert alert;
    private EnemyChase chase;

    



    private void Awake()
    {
        
        detection = GetComponent<EnemyDetection>();
        patrol = GetComponent<EnemyPatrol>();
        chase = GetComponent<EnemyChase>();
        alert = GetComponent<EnemyAlert>();
        wasChasingPlayer = false;
    }

    private void Update()
    {
        UpdateBehaviourState();
    }  

    private void UpdateBehaviourState() 
    {
        playerTransform = detection.DetectTarget();

      
        if (playerTransform != null)
        {
            StopAllCoroutines();
            wasChasingPlayer = true;
            lastKnownPlayerPosition = playerTransform.position;
            chase.PerformChase(playerTransform);
            alert.ClearAlert();
            return;
        }
        if (isSearching)
        {
            return;
        }

        if (alert.HasAlert())
        {
            alert.PerformInspection();
            return;
        }
        if (playerTransform == null && wasChasingPlayer)
        {
            wasChasingPlayer = false;
            isSearching = true;
            StartCoroutine(SearchForPlayer(lastKnownPlayerPosition));
            return;
        }

        patrol.PerformPatrol();
            
        

    }

    public IEnumerator SearchForPlayer(Vector3 lastKnownPlayerPosition)
    {
        

        navAgent.SetDestination(lastKnownPlayerPosition);

        while (navAgent.pathPending ||
               navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }

        yield return StartCoroutine(WaitWithDetection(3));

        isSearching = false;
        navAgent.isStopped = false;
        navAgent.ResetPath();

    }

    public IEnumerator WaitWithDetection(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (detection.DetectTarget() != null)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
