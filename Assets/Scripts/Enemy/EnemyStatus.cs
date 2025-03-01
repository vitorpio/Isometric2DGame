using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    Rigidbody rb;

    int health = 3;
    public bool isTakingDamage = false;
    readonly float timeBetweenDamage = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(Vector3 playerPosition, int damage, float attackForce)
    {
        isTakingDamage = true;
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            ApplyDamageForce(playerPosition, attackForce);
            Invoke(nameof(RecoverFromDamage), timeBetweenDamage);
        }
    }

    void ApplyDamageForce(Vector3 playerPosition, float attackForce)
    {
        Vector3 vectorForce = new Vector3(transform.position.x - playerPosition.x, 0f, transform.position.z - playerPosition.z) * attackForce;
        rb.AddForce(vectorForce, ForceMode.Impulse);
    }

    void RecoverFromDamage()
    {
        rb.linearVelocity = Vector3.zero;
        isTakingDamage = false;
    }
}
