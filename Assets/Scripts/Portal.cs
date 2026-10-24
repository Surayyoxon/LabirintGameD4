using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform exitPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            other.transform.position = exitPoint.position;
            other.transform.rotation = exitPoint.rotation;
            controller.enabled = true;
        }
        else
        {
            other.transform.position = exitPoint.position;
            other.transform.rotation = exitPoint.rotation;
        }
    }
}