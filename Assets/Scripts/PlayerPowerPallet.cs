using UnityEngine;
using System.Collections;

public class PlayerPowerPallet : MonoBehaviour
{
    [Header("Power Settings")]
    [SerializeField] private float powerDuration = 15f;

    private bool isPoweredUp = false;

    private float powerEndTime;

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

        powerEndTime =
            Time.time + powerDuration;

        Debug.Log("POWER MODE ON!");

        yield return new WaitForSeconds(powerDuration);

        isPoweredUp = false;

        powerEndTime = 0f;

        Debug.Log("POWER MODE OFF!");

        powerCoroutine = null;
    }


    public bool IsPoweredUp()
    {
        return isPoweredUp;
    }


    public float GetRemainingPowerTime()
    {
        if (!isPoweredUp)
            return 0f;

        return Mathf.Max(
            0f,
            powerEndTime - Time.time
        );
    }


    public float GetPowerDuration()
    {
        return powerDuration;
    }
}