using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyBController : MonoBehaviour
{
    private enum State
    {
        Guard,
        Chase,
        Return
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Guard")]
    [Tooltip("Bo'sh qoldirilsa, enemy o'yin boshidagi joyini guard nuqta qilib oladi")]
    [SerializeField] private Transform guardPoint;
    [SerializeField] private float guardRadius = 5f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float pointReachDistance = 0.3f;
    [SerializeField] private float gravity = -20f;

    [Header("Chase")]
    [SerializeField] private float chaseTimeMin = 20f;
    [SerializeField] private float chaseTimeMax = 30f;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackDamage = 5f;
    [SerializeField] private float attackInterval = 1f;
    [Tooltip("Attack holatidan chiqish uchun qo'shimcha masofa (animatsiya titrashini oldini oladi)")]
    [SerializeField] private float attackExitBuffer = 0.7f;

    [Header("Animator Parameters")]
    [SerializeField] private string walkingParameter = "IsWalking";
    [SerializeField] private string attackingParameter = "IsAttacking";

    private CharacterController controller;

    private State currentState = State.Guard;

    // Guard nuqta (world position sifatida saqlanadi)
    private Vector3 homePosition;

    // Shu frame uchun gorizontal harakat va vertikal tezlik
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private float chaseEndTime;
    private float nextAttackTime;

    // Hozir attack holatidami (hysteresis uchun)
    private bool isInAttackRange;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Guard nuqta: biriktirilgan bo'lsa shu, bo'lmasa boshlang'ich joy
        homePosition = guardPoint != null
            ? guardPoint.position
            : transform.position;

        SetWalking(false);
        SetAttacking(false);
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        horizontalVelocity = Vector3.zero;

        if (player != null)
        {
            switch (currentState)
            {
                case State.Guard:
                    UpdateGuard();
                    break;

                case State.Chase:
                    UpdateChase();
                    break;

                case State.Return:
                    UpdateReturn();
                    break;
            }
        }

        ApplyMovement();
    }

    // =====================================================
    // APPLY MOVEMENT (devorlarga to'qnashadi)
    // =====================================================

    private void ApplyMovement()
    {
        if (controller == null || !controller.enabled)
            return;

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = horizontalVelocity;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    // =====================================================
    // GUARD
    // =====================================================

    private void UpdateGuard()
    {
        SetWalking(false);
        SetAttacking(false);

        // Player guard radiusiga kirdimi (y hisobga olinmaydi)
        float distanceToPlayer = FlatDistance(homePosition, player.position);

        if (distanceToPlayer <= guardRadius)
        {
            StartChase();
        }
    }

    // =====================================================
    // CHASE
    // =====================================================

    private void StartChase()
    {
        currentState = State.Chase;

        float chaseDuration = Random.Range(chaseTimeMin, chaseTimeMax);
        chaseEndTime = Time.time + chaseDuration;

        Debug.Log("EnemyB CHASE boshladi: " + chaseDuration + " sekund");
    }

    private void UpdateChase()
    {
        // Chase vaqti tugadi
        if (Time.time >= chaseEndTime)
        {
            StartReturn();
            return;
        }

        float distanceToPlayer = FlatDistance(transform.position, player.position);

        // Hysteresis: attackka kirish attackDistance da,
        // chiqish esa attackDistance + buffer da (walk/attack almashinuvi titramaydi)
        if (isInAttackRange)
        {
            if (distanceToPlayer > attackDistance + attackExitBuffer)
                isInAttackRange = false;
        }
        else if (distanceToPlayer <= attackDistance)
        {
            isInAttackRange = true;
        }

        if (isInAttackRange)
        {
            Attack();
            return;
        }

        // Playerni quvish
        MoveTowards(player.position, chaseSpeed);

        SetWalking(true);
        SetAttacking(false);
    }

    // =====================================================
    // ATTACK
    // =====================================================

    private void Attack()
    {
        SetWalking(false);
        SetAttacking(true);

        RotateTowards(player.position);

        // Player Power Mode'da bo'lsa damage bermaydi
        PlayerPowerPallet power = player.GetComponent<PlayerPowerPallet>();

        if (power != null && power.IsPoweredUp())
            return;

        // Oddiy attack
        if (Time.time >= nextAttackTime)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }

            nextAttackTime = Time.time + attackInterval;
        }
    }

    // =====================================================
    // RETURN
    // =====================================================

    private void StartReturn()
    {
        currentState = State.Return;
        isInAttackRange = false;

        Debug.Log("EnemyB HOME ga qaytmoqda");
    }

    private void UpdateReturn()
    {
        float distanceToHome = FlatDistance(transform.position, homePosition);

        // Guard nuqtaga yetib keldi
        if (distanceToHome <= pointReachDistance)
        {
            currentState = State.Guard;

            SetWalking(false);
            SetAttacking(false);

            Debug.Log("EnemyB HOME ga qaytdi");
            return;
        }

        MoveTowards(homePosition, returnSpeed);

        SetWalking(true);
        SetAttacking(false);
    }

    // =====================================================
    // MOVEMENT HELPERS
    // =====================================================

    private void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;

        // Faqat gorizontal tekislikda harakat
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();

        // Haqiqiy harakat ApplyMovement() da CharacterController orqali bo'ladi
        horizontalVelocity = direction * speed;

        RotateTowards(target);
    }

    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;

        return Vector3.Distance(a, b);
    }

    // =====================================================
    // ANIMATION
    // =====================================================

    private void SetWalking(bool value)
    {
        if (animator != null)
            animator.SetBool(walkingParameter, value);
    }

    private void SetAttacking(bool value)
    {
        if (animator != null)
            animator.SetBool(attackingParameter, value);
    }

    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying
            ? homePosition
            : (guardPoint != null ? guardPoint.position : transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, guardRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}