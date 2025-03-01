using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    Status status;
    Animator animator;
    Rigidbody rb;

    [SerializeField] InputAction move;

    // Movement params
    readonly float movementSpeed = 5f;
    readonly float maxSpeed = 8f;
    readonly string moveUpOrDownAnimation = "IsMovingUpOrDown";
    readonly string moveLeftOrRightAnimation = "IsMovingLeftOrRight";

    void Awake()
    {
        status = GetComponent<Status>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        move.Enable();
    }

    void OnDisable()
    {
        move.Disable();
    }

    void Update()
    {
        ProcessMovement();
    }

    void ProcessMovement()
    {
        if (status.isTakingDamage)
        {
            return;
        }

        Vector2 movement = move.ReadValue<Vector2>();

        // Disable diagonal movement
        if (Mathf.Abs(movement.x) > 0 && Mathf.Abs(movement.y) > 0)
        {
            movement = Vector2.zero;
        }

        if (movement != Vector2.zero)
        {
            ApplyMovement(movement);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        ProcessMovementAnimation(movement);
    }

    void ApplyMovement(Vector2 movement)
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            Vector3 force = new Vector3(movement.x, 0, movement.y) * movementSpeed;
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    void ProcessMovementAnimation(Vector2 movement)
    {
        bool isMovingUpOrDown = movement.y != 0;
        bool isMovingLeftOrRight = movement.x != 0;

        animator.SetBool(moveUpOrDownAnimation, isMovingUpOrDown);
        animator.SetBool(moveLeftOrRightAnimation, isMovingLeftOrRight);

        if (!isMovingUpOrDown && !isMovingLeftOrRight)
        {
            StopMovement();
        }
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        animator.SetBool(moveUpOrDownAnimation, false);
        animator.SetBool(moveLeftOrRightAnimation, false);
    }
}
