using System.Collections;
using UnityEngine;

public class StateAI : MonoBehaviour
{
    Animator animator;

    private State _currentState;
    private State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                animator.SetTrigger(value.ToString());
                if (checkPatrolCoroutine != null)
                {
                    StopCoroutine(checkPatrolCoroutine);
                    checkPatrolCoroutine = null;
                }
            }
        }
    }

    // Patrolling params
    Vector3[] patrolPoints;
    Coroutine checkPatrolCoroutine;
    int nextPatrolPointIndex = 0;
    readonly float patrollingSpeed = 3f;
    readonly float minDistanceToPatrolPoint = 0.1f;
    readonly float timeToCheckPatrol = 1f;
    readonly int minPatrolPoints = 2;
    readonly int maxPatrolPoints = 5;
    readonly float minPatrolPointDistance = -10f;
    readonly float maxPatrolPointDistance = 10f;

    // Chase params
    Vector3? chasedPlayerPosition;
    readonly float chassingSpeed = 3.5f;

    // Attack params
    GameObject playerBeingAttacked;

    // Other Variables
    readonly string playerTag = "Player";

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        GenerateRandomPatrolRoute();
    }

    void Update()
    {
        ProcessState();
    }

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
            CurrentState = State.Attack;
            playerBeingAttacked = other.gameObject;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            CurrentState = State.Chase;
            playerBeingAttacked = null;
        }
    }

    void ProcessState()
    {
        switch (CurrentState)
        {
            case State.Idle:
                CheckIfWillPatrol();
                break;
            case State.Patrol:
                MoveToNextPatrolPoint();
                break;
            case State.Chase:
                ChasePlayer();
                break;
            case State.Attack:
                AttackPlayer();
                break;
        }
    }

    void CheckIfWillPatrol()
    {
        checkPatrolCoroutine ??= StartCoroutine(CheckPatrolRoutine());
    }

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
        if (Vector3.Distance(transform.parent.position, patrolPoints[nextPatrolPointIndex]) < minDistanceToPatrolPoint)
        {
            nextPatrolPointIndex = (nextPatrolPointIndex + 1) % patrolPoints.Length;
        }

        transform.parent.position = Vector3.MoveTowards(transform.parent.position, patrolPoints[nextPatrolPointIndex], patrollingSpeed * Time.deltaTime);
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

    void GenerateRandomPatrolRoute()
    {
        int patrolPointsCount = Random.Range(minPatrolPoints, maxPatrolPoints);
        patrolPoints = new Vector3[patrolPointsCount];
        for (int i = 0; i < patrolPointsCount; i++)
        {
            patrolPoints[i] = new Vector3(transform.parent.position.x + Random.Range(minPatrolPointDistance, maxPatrolPointDistance), transform.parent.position.y, Random.Range(minPatrolPointDistance, maxPatrolPointDistance));
        }
    }
}
