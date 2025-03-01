using UnityEngine;

public class Status : MonoBehaviour
{
    Rigidbody rb;
    Movement movement;

    int health = 10;
    public bool isTakingDamage = false;
    readonly float timeBetweenDamage = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (isTakingDamage)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void TakeDamage(Vector3 enemyPosition, int damage, float attackForce)
    {
        isTakingDamage = true;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            movement.StopMovement();
            Vector3 vectorForce = new Vector3(transform.position.x - enemyPosition.x, 0f, transform.position.z - enemyPosition.z) * attackForce;
            rb.AddForce(vectorForce, ForceMode.Impulse);
            Invoke(nameof(RecoverFromDamage), timeBetweenDamage);
        }
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void RecoverFromDamage()
    {
        isTakingDamage = false;
    }
}
