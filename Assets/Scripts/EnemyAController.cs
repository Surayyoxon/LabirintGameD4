using UnityEngine;
using System.Collections;

public class EnemyAController : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float pointReachDistance = 0.15f;

    [Header("Attack")]
    [SerializeField] private Transform player;
    [SerializeField] private float attackDistance = 2.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Animator Parameters")]
    [SerializeField] private string walkingParameter = "IsWalking";
    [SerializeField] private string attackingParameter = "IsAttacking";

    [Header("Attack Damage")]
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackInterval = 1f;

    private float nextAttackTime;
    private Transform currentTarget;
    private bool isWaiting;
    private Coroutine waitCoroutine;

    private void Start()
    {
        // Dastlab B nuqtaga yuradi
        currentTarget = pointB;

        SetWalking(false);
        SetAttacking(false);
    }

    private void Update()
    {
        // Player biriktirilmagan bo'lsa patrol davom etadi
        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // Player attack radius ichida
        if (distanceToPlayer <= attackDistance)
        {
            Attack();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // A yoki B nuqtada kutish
        if (isWaiting)
        {
            SetWalking(false);
            SetAttacking(false);
            return;
        }

        if (currentTarget == null)
            return;

        // Hozirgi nuqtagacha masofa
        float distance = Vector3.Distance(
            transform.position,
            currentTarget.position
        );

        // Nuqtaga yetib keldi
        if (distance <= pointReachDistance)
        {
            // Nuqtaga aniq joylashish
            transform.position = currentTarget.position;

            SetWalking(false);
            SetAttacking(false);

            waitCoroutine = StartCoroutine(WaitAtPoint());

            return;
        }

        // Target tomon yo'nalish
        Vector3 direction =
            (currentTarget.position - transform.position).normalized;

        // Harakat
        transform.position +=
            direction * moveSpeed * Time.deltaTime;

        // Enemy yurish yo'nalishiga qaraydi
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        // Animation
        SetWalking(true);
        SetAttacking(false);
    }

    private void Attack()
    {
        SetWalking(false);
        SetAttacking(true);

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        // Player Power Mode'da bo'lsa damage bermaydi
        PlayerPowerPallet power =
            player.GetComponent<PlayerPowerPallet>();

        if (power != null && power.IsPoweredUp())
        {
            return;
        }

        // Oddiy attack
        if (Time.time >= nextAttackTime)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            nextAttackTime = Time.time + attackInterval;
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        // Idle
        SetWalking(false);
        SetAttacking(false);

        yield return new WaitForSeconds(waitTime);

        // A <-> B
        if (currentTarget == pointA)
        {
            currentTarget = pointB;
        }
        else
        {
            currentTarget = pointA;
        }

        isWaiting = false;
        waitCoroutine = null;
    }

    private void SetWalking(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(walkingParameter, value);
        }
    }

    private void SetAttacking(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(attackingParameter, value);
        }
    }

    private void OnDisable()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }
}