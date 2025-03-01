using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    Animator animator;

    [SerializeField] InputAction attack;

    public bool isAttacking = false;
    readonly float timeBetweenAttacks = 1f;
    readonly string attackAnimation = "Attack";

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
