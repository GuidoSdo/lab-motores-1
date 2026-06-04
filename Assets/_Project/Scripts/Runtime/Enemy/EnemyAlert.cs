using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyAlert : MonoBehaviour
{
    private EnemyBehaviour behaviour;
    [SerializeField] private Transform alertedTransform;
    [SerializeField] private NavMeshAgent navAgent;
    private GameObject alertObj;
    private bool alertDetected;
    private bool isInvestigatingAlert;


    private void Awake()
    {
        alertDetected = false;
        alertObj = GameObject.Find("Alert");
        isInvestigatingAlert = false;
        behaviour = GetComponent<EnemyBehaviour>();

    }
    private Transform CheckAlert()
    {
        Transform transform = alertObj.transform;

        return transform;


    }
    private IEnumerator InvestigateAlert()
    {

        navAgent.isStopped = true;

        yield return StartCoroutine(behaviour.WaitWithDetection(3));


        alertDetected = false;

        navAgent.isStopped = false;
        isInvestigatingAlert = false;
        navAgent.ResetPath();
    }

    public void PerformInspection()
    {
        if (isInvestigatingAlert) return;

        alertedTransform = CheckAlert();
        navAgent.SetDestination(alertedTransform.position);

        if (!navAgent.pathPending &&
            navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            
            isInvestigatingAlert = true;
            StartCoroutine(InvestigateAlert());
        }
    }

    public bool HasAlert() 
    {
        return alertDetected || isInvestigatingAlert;
    }

    public void ClearAlert() 
    {
        alertDetected = false;
    }
}
