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
    readonly float chassingSpeed = 3.5f;
    GameObject playerBeingChased;

    // Attack params
    GameObject playerBeingAttacked;
    readonly float attackForce = 2f;
    readonly int damage = 1;

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
            playerBeingChased = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CurrentState = State.Idle;
            playerBeingChased = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerBeingChased = other.gameObject;
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

    // Coroutine to check if the enemy will patrol chance is 50/50
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
        // If the enemy is close enough to the next patrol point, move to the next one
        if (Vector3.Distance(transform.parent.position, patrolPoints[nextPatrolPointIndex]) < minDistanceToPatrolPoint)
        {
            nextPatrolPointIndex = (nextPatrolPointIndex + 1) % patrolPoints.Length;
        }

        transform.parent.position = Vector3.MoveTowards(transform.parent.position, patrolPoints[nextPatrolPointIndex], patrollingSpeed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        if (playerBeingChased != null && !playerBeingChased.GetComponent<Status>().isTakingDamage)
        {
            Vector3 chasedPlayerPosition = playerBeingChased.transform.position;
            Vector3 targetPositionXZ = new(chasedPlayerPosition.x, transform.parent.position.y, chasedPlayerPosition.z);
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, targetPositionXZ, chassingSpeed * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        Attack attack = playerBeingAttacked.GetComponent<Attack>();
        if (!attack.isAttacking && !playerBeingAttacked.GetComponent<Status>().isTakingDamage)
        {
            Status playerStatus = playerBeingAttacked.GetComponent<Status>();
            if (!playerStatus.isTakingDamage)
            {
                playerStatus.TakeDamage(transform.parent.position, damage, attackForce);
            }
        }
    }

    // Generate a random patrol route for the enemy
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
