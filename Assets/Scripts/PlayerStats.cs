using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int BetAmount { get; private set; }
    public int CurrentBetIndex = 0;
    public int HandsPlayed = 0;
    public bool DidGameJustStart { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBetAmount(int value)
    {
        BetAmount = value;
    }

    /// <summary>
    /// This checks to false if a game was played atlease once
    /// </summary>
    public void GameStartedOnce()
    {
        DidGameJustStart = false;
    }
}
