using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    Animator animator;

    [SerializeField] InputAction attack;

    public bool isAttacking = false;
    readonly float timeBetweenAttacks = 1f;
    readonly string attackAnimation = "Attack";
    readonly int attackDamage = 1;
    readonly float attackForce = 3f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        attack.Enable();
    }

    void OnDisable()
    {
        attack.Disable();
    }

    void Update()
    {
        ProcessAttack();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") && isAttacking)
        {
            EnemyStatus enemyStatus = other.gameObject.GetComponentInParent<EnemyStatus>();
            if (!enemyStatus.isTakingDamage)
                enemyStatus.TakeDamage(transform.position, attackDamage, attackForce);
        }
    }

    void ProcessAttack()
    {
        if (attack.triggered && !isAttacking)
        {
            isAttacking = true;

            animator.SetTrigger(attackAnimation);

            Invoke(nameof(FinishAttack), timeBetweenAttacks);
        }
    }

    void FinishAttack()
    {
        isAttacking = false;
    }
}
