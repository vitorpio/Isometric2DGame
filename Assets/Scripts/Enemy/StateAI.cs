using System.Collections;
using UnityEngine;

public class StateAI : MonoBehaviour
{
    Animator animator;
    EnemyStatus enemyStatus;

    private State _currentState;
    private State CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState != value)
            {
                if (value == State.Attack && playerBeingAttacked != null && playerBeingAttacked.GetComponent<Attack>().isAttacking)
                {
                    return;
                }
                _currentState = value;
                animator.SetTrigger(value.ToString());
                StopCheckPatrolCoroutine();
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
        animator = GetComponent<Animator>();
        enemyStatus = GetComponent<EnemyStatus>();
        GenerateRandomPatrolRoute();
    }

    void Update()
    {
        if (!enemyStatus.isTakingDamage)
        {
            ProcessState();
        }
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
        if (checkPatrolCoroutine == null)
        {
            checkPatrolCoroutine = StartCoroutine(CheckPatrolRoutine());
        }
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
        if (Vector3.Distance(transform.position, patrolPoints[nextPatrolPointIndex]) < minDistanceToPatrolPoint)
        {
            nextPatrolPointIndex = (nextPatrolPointIndex + 1) % patrolPoints.Length;
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[nextPatrolPointIndex], patrollingSpeed * Time.deltaTime);
    }

    void ChasePlayer()
    {
        if (playerBeingChased != null && !playerBeingChased.GetComponent<Status>().isTakingDamage)
        {
            Vector3 chasedPlayerPosition = playerBeingChased.transform.position;
            Vector3 targetPositionXZ = new(chasedPlayerPosition.x, transform.position.y, chasedPlayerPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPositionXZ, chassingSpeed * Time.deltaTime);
        }
    }

    void AttackPlayer()
    {
        Attack attack = playerBeingAttacked.GetComponent<Attack>();
        if (!attack.isAttacking)
        {
            Status playerStatus = playerBeingAttacked.GetComponent<Status>();
            if (!playerStatus.isTakingDamage)
            {
                playerStatus.TakeDamage(transform.position, damage, attackForce);
            }
        }
    }

    void GenerateRandomPatrolRoute()
    {
        int patrolPointsCount = Random.Range(minPatrolPoints, maxPatrolPoints);
        patrolPoints = new Vector3[patrolPointsCount];
        for (int i = 0; i < patrolPointsCount; i++)
        {
            patrolPoints[i] = new Vector3(transform.position.x + Random.Range(minPatrolPointDistance, maxPatrolPointDistance), transform.position.y, Random.Range(minPatrolPointDistance, maxPatrolPointDistance));
        }
    }

    void StopCheckPatrolCoroutine()
    {
        if (checkPatrolCoroutine != null)
        {
            StopCoroutine(checkPatrolCoroutine);
            checkPatrolCoroutine = null;
        }
    }
}
