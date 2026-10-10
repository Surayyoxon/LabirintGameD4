using UnityEngine;
using System.Collections;

public class PlayerPowerPallet : MonoBehaviour
{
    [Header("Power Pellet Settings")]
    [SerializeField] private float powerDuration = 15f;

    private bool isPoweredUp = false;
    private Coroutine powerCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PowerPallet"))
            return;

        ActivatePower();

        Destroy(other.gameObject);
    }

    private void ActivatePower()
    {
        if (powerCoroutine != null)
        {
            StopCoroutine(powerCoroutine);
        }

        powerCoroutine = StartCoroutine(PowerMode());
    }

    private IEnumerator PowerMode()
    {
        isPoweredUp = true;

        Debug.Log("POWER MODE ON!");

        yield return new WaitForSeconds(powerDuration);

        isPoweredUp = false;

        Debug.Log("POWER MODE OFF!");

        powerCoroutine = null;
    }

    public bool IsPoweredUp()
    {
        return isPoweredUp;
    }
}
