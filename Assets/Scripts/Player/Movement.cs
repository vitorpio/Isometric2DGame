using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] InputAction move;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float maxSpeed = 8f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        ProcessMovement();
    }

    void ProcessMovement()
    {
        Vector2 movement = move.ReadValue<Vector2>();

        if (movement != Vector2.zero)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                Vector3 force = new Vector3(movement.y, 0, -movement.x) * movementSpeed;
                rb.AddForce(force, ForceMode.Acceleration);
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    void OnEnable()
    {
        move.Enable();
    }

    void OnDisable()
    {
        move.Disable();
    }
}
