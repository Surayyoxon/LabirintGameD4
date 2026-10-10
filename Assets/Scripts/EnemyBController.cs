using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private Transform guardPoint;

    [Header("Guard")]
    [SerializeField] private float guardRadius = 5f;

    [Header("Chase")]
    [SerializeField] private float chaseTimeMin = 20f;
    [SerializeField] private float chaseTimeMax = 30f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackInterval = 1f;

    [Header("Animator Parameters")]
    [SerializeField] private string walkingParameter = "IsWalking";
    [SerializeField] private string attackingParameter = "IsAttacking";

    private bool isChasing;
    private bool isReturning;

    private float chaseEndTime;
    private float nextAttackTime;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (guardPoint == null)
            guardPoint = transform;

        // Enemy NavMesh ustida turibdimi?
        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "EnemyB NavMesh ustida emas!"
            );

            return;
        }

        agent.isStopped = true;

        SetWalking(false);
        SetAttacking(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        // CHASE
        if (isChasing)
        {
            UpdateChase();
            return;
        }

        // RETURN
        if (isReturning)
        {
            UpdateReturn();
            return;
        }

        // GUARD
        UpdateGuard();
    }

    // =====================================================
    // GUARD
    // =====================================================

    private void UpdateGuard()
    {
        agent.isStopped = true;

        SetWalking(false);
        SetAttacking(false);

        float distanceToPlayer = Vector3.Distance(
            guardPoint.position,
            player.position
        );

        // Player guard radius ichiga kirdi
        if (distanceToPlayer <= guardRadius)
        {
            StartChase();
        }
    }

    // =====================================================
    // START CHASE
    // =====================================================

    private void StartChase()
    {
        if (isChasing || isReturning)
            return;

        isChasing = true;

        agent.isStopped = false;

        // 20-30 sekundlik random chase
        float chaseDuration = Random.Range(
            chaseTimeMin,
            chaseTimeMax
        );

        chaseEndTime = Time.time + chaseDuration;

        Debug.Log(
            "EnemyB CHASE boshladi: "
            + chaseDuration + " sekund"
        );
    }

    // =====================================================
    // CHASE
    // =====================================================

    private void UpdateChase()
    {
        // Chase vaqti tugadi
        if (Time.time >= chaseEndTime)
        {
            StartReturn();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // Attack
        if (distanceToPlayer <= attackDistance)
        {
            Attack();
            return;
        }

        // Playerni quvish
        agent.isStopped = false;

        agent.SetDestination(player.position);

        SetWalking(true);
        SetAttacking(false);
    }

    // =====================================================
    // ATTACK
    // =====================================================

    private void Attack()
    {
        agent.isStopped = true;

        SetWalking(false);
        SetAttacking(true);

        Vector3 direction =
            player.position - transform.position;

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
            PlayerHealth health =
                player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(attackDamage);

                Debug.Log(
                    "EnemyB Playerga damage berdi: "
                    + attackDamage
                );
            }

            nextAttackTime =
                Time.time + attackInterval;
        }
    }

    // =====================================================
    // START RETURN
    // =====================================================

    private void StartReturn()
    {
        isChasing = false;
        isReturning = true;

        agent.isStopped = false;

        SetWalking(true);
        SetAttacking(false);

        agent.SetDestination(
            guardPoint.position
        );

        Debug.Log("EnemyB HOME ga qaytmoqda");
    }

    // =====================================================
    // RETURN
    // =====================================================

    private void UpdateReturn()
    {
        agent.isStopped = false;

        agent.SetDestination(
            guardPoint.position
        );

        SetWalking(true);
        SetAttacking(false);

        // Guard pointga yetib keldi
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            FinishReturn();
        }
    }

    // =====================================================
    // FINISH RETURN
    // =====================================================

    private void FinishReturn()
    {
        isReturning = false;

        agent.isStopped = true;

        SetWalking(false);
        SetAttacking(false);

        Debug.Log("EnemyB HOME ga qaytdi");
    }

    // =====================================================
    // ANIMATION
    // =====================================================

    private void SetWalking(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(
                walkingParameter,
                value
            );
        }
    }

    private void SetAttacking(bool value)
    {
        if (animator != null)
        {
            animator.SetBool(
                attackingParameter,
                value
            );
        }
    }

    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (guardPoint == null)
            return;

        Gizmos.DrawWireSphere(
            guardPoint.position,
            guardRadius
        );
    }
}