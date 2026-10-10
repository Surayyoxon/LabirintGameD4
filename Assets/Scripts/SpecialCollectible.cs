using UnityEngine;
using System.Collections;

public class SpecialCollectible : MonoBehaviour
{
    [SerializeField] private float visibleTime = 5f;
    [SerializeField] private float hiddenTime = 5f;

    private Collider collectibleCollider;
    private Renderer[] renderers;

    private void Start()
    {
        collectibleCollider = GetComponent<Collider>();
        renderers = GetComponentsInChildren<Renderer>();

        StartCoroutine(VisibilityCycle());
    }

    private IEnumerator VisibilityCycle()
    {
        while (true)
        {
            SetVisible(true);
            yield return new WaitForSeconds(visibleTime);

            SetVisible(false);
            yield return new WaitForSeconds(hiddenTime);
        }
    }

    private void SetVisible(bool visible)
    {
        collectibleCollider.enabled = visible;

        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }
    }
}
