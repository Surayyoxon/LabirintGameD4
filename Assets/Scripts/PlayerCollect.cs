using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    private int score = 0;
    private int simpleScore = 0;
    private int bonusScore = 0;

    [Header("Collectible Tags")]
    [SerializeField] private string normalCollectibleTag = "Collectible";
    [SerializeField] private string specialCollectibleTag = "SpecialCollectible";

    private void OnTriggerEnter(Collider other)
    {
        // Oddiy collectible
        if (other.CompareTag(normalCollectibleTag))
        {
            score += 1;
            simpleScore += 1;

            Debug.Log("Oddiy collectible olindi! +1");
            Debug.Log("Simple Score: " + simpleScore);
            Debug.Log("All Score: " + score);

            Destroy(other.gameObject);
        }

        // Bonus collectible
        else if (other.CompareTag(specialCollectibleTag))
        {
            score += 5;
            bonusScore += 1;

            Debug.Log("Bonus collectible olindi! +5");
            Debug.Log("Bonus Score: " + bonusScore);
            Debug.Log("All Score: " + score);

            Destroy(other.gameObject);
        }
    }

    public int GetScore()
    {
        return score;
    }

    public int GetSimpleScore()
    {
        return simpleScore;
    }

    public int GetBonusScore()
    {
        return bonusScore;
    }
}