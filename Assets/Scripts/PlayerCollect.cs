using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    private int score = 0;

    [Header("Collectible Tags")]
    [SerializeField] private string normalCollectibleTag = "Collectible";
    [SerializeField] private string specialCollectibleTag = "SpecialCollectible";

    private void OnTriggerEnter(Collider other)
    {
        // Oddiy collectible = +1
        if (other.CompareTag(normalCollectibleTag))
        {
            score += 1;

            Debug.Log("Oddiy collectible olindi! +1 Score");
            Debug.Log("Current Score: " + score);

            Destroy(other.gameObject);
        }

        // Maxsus collectible = +5
        else if (other.CompareTag(specialCollectibleTag))
        {
            score += 5;

            Debug.Log("Maxsus collectible olindi! +5 Score");
            Debug.Log("Current Score: " + score);

            Destroy(other.gameObject);
        }
    }

    public int GetScore()
    {
        return score;
    }
}
