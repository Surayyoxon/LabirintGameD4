using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float lookHeight = 1.5f;

    private Vector3 localOffset;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow: Player ulanmagan!");
            enabled = false;
            return;
        }

        // Kameraning siz joylashtirgan boshlang‘ich masofasini saqlaydi
        localOffset = Quaternion.Inverse(player.rotation) *
                      (transform.position - player.position);
    }

    private void LateUpdate()
    {
        FollowPlayer();
        LookAtPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 desiredPosition =
            player.position + player.rotation * localOffset;

        float smoothValue =
            1f - Mathf.Exp(-followSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothValue
        );
    }

    private void LookAtPlayer()
    {
        Vector3 lookPoint =
            player.position + Vector3.up * lookHeight;

        Vector3 lookDirection =
            lookPoint - transform.position;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion desiredRotation =
            Quaternion.LookRotation(lookDirection);

        float smoothValue =
            1f - Mathf.Exp(-rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            smoothValue
        );
    }
}
