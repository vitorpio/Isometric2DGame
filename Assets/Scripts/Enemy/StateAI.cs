using System.Collections;
using UnityEngine;

public class StateAI : MonoBehaviour
{
    public State CurrentState = State.Idle;

    // Patrolling params
    [SerializeField] GameObject[] patrolPoints;
    int nextPatrolPointIndex = 0;
    Coroutine checkPatrolCoroutine;
    readonly float patrollingSpeed = 3f;
    readonly float minDistanceToPatrolPoint = 0.1f;
    readonly float timeToCheckPatrol = 1f;

    // Chase params
    readonly float chassingSpeed = 3.5f;
    Vector3? chasedPlayerPosition;

    // Attack params
    GameObject playerBeingAttacked;

    private readonly string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CurrentState = State.Chase;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CurrentState = State.Idle;
            chasedPlayerPosition = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            chasedPlayerPosition = other.transform.position;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            playerBeingAttacked = other.gameObject;
            CurrentState = State.Attack;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            playerBeingAttacked = null;
            CurrentState = State.Chase;
        }
    }

    void Update()
    {
        ProcessState();
    }

    void ProcessState()
    {
        if (CurrentState == State.Idle)
        {
            CheckIfWillPatrol();
        }
        else if (CurrentState == State.Patrol)
        {
            MoveToNextPatrolPoint();
        }
        else if (CurrentState == State.Chase)
        {
            ChasePlayer();
        }
        else if (CurrentState == State.Attack)
        {
            AttackPlayer();
        }
    }

    // Check if will patrol after some time
    void CheckIfWillPatrol()
    {
        if (checkPatrolCoroutine != null)
        {
            return;
        }
        checkPatrolCoroutine = StartCoroutine(nameof(CheckPatrolRoutine));
    }

    // Coroutine to check every second if will patrol the chance to siwtch to patrol is 50/50
    IEnumerator CheckPatrolRoutine()
    {
        yield return new WaitForSeconds(timeToCheckPatrol);
        if (Random.value > 0.5f && CurrentState == State.Idle)
        {
            CurrentState = State.Patrol;
        }
        checkPatrolCoroutine = null;
    }

    void MoveToNextPatrolPoint()
    {
        // Check if reached patrol point move to the next one
        if (Vector3.Distance(transform.parent.position, patrolPoints[nextPatrolPointIndex].transform.position) < minDistanceToPatrolPoint)
        {
            nextPatrolPointIndex = (nextPatrolPointIndex + 1) % patrolPoints.Length;
        }

        transform.parent.position = Vector3.MoveTowards(transform.parent.position, patrolPoints[nextPatrolPointIndex].transform.position, patrollingSpeed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        if (chasedPlayerPosition.HasValue)
        {
            Vector3 targetPositionXZ = new(chasedPlayerPosition.Value.x, transform.parent.position.y, chasedPlayerPosition.Value.z);
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, targetPositionXZ, chassingSpeed * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Attacking player");
    }

}
