using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    Rigidbody rb;
    Animator animator;

    int health = 3;
    public bool isTakingDamage = false;
    readonly float timeBetweenDamage = 0.5f;
    readonly float deathAnimationTime = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(Vector3 playerPosition, int damage, float attackForce)
    {
        isTakingDamage = true;
        health -= damage;
        if (health <= 0)
        {
            animator.SetTrigger("Die");
            Invoke(nameof(DestroyEnemy), deathAnimationTime);
        }
        else
        {
            ApplyDamageForce(playerPosition, attackForce);
            Invoke(nameof(RecoverFromDamage), timeBetweenDamage);
        }
    }

    void DestroyEnemy()
    {
        Destroy(gameObject);
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
