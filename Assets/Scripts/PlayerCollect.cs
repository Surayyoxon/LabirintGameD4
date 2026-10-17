using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    private int allScore = 0;
    private int simpleScore = 0;
    private int bonusScore = 0;

    [Header("Collectible Tags")]
    [SerializeField] private string normalCollectibleTag = "Collectible";
    [SerializeField] private string specialCollectibleTag = "SpecialCollectible";


    private void OnTriggerEnter(Collider other)
    {
        // ==========================================
        // ODDIY COLLECTIBLE
        // ==========================================

        if (other.CompareTag(normalCollectibleTag))
        {
            allScore += 1;
            simpleScore += 1;

            Debug.Log("Oddiy collectible olindi! +1");
            Debug.Log("All Score: " + allScore);
            Debug.Log("Simple Score: " + simpleScore);

            Destroy(other.gameObject);
        }


        // ==========================================
        // BONUS COLLECTIBLE
        // ==========================================

        else if (other.CompareTag(specialCollectibleTag))
        {
            allScore += 5;
            bonusScore += 1;

            Debug.Log("Bonus collectible olindi! +5");
            Debug.Log("All Score: " + allScore);
            Debug.Log("Bonus Score: " + bonusScore);

            Destroy(other.gameObject);
        }
    }


    // ==========================================
    // GET ALL SCORE
    // ==========================================

    public int GetScore()
    {
        return allScore;
    }


    // ==========================================
    // GET SIMPLE SCORE
    // ==========================================

    public int GetSimpleScore()
    {
        return simpleScore;
    }


    // ==========================================
    // GET BONUS SCORE
    // ==========================================

    public int GetBonusScore()
    {
        return bonusScore;
    }
}