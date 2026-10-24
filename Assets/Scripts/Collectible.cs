using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collect VFX")]
    [SerializeField] private ParticleSystem collectVFX;

    [Header("Collect Audio")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // VFX
        if (collectVFX != null)
        {
            Instantiate(
                collectVFX,
                transform.position,
                Quaternion.identity
            );
        }

        // Audio
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position,
                volume
            );
        }

        // Collectible'ni o'chirish
        Destroy(gameObject);
    }
}