using System.Collections;
using TMPro;
using UnityEngine;

public class ShuffleCard : MonoBehaviour
{
    public GameObject[] cards; // Assign your buttons (cards) in the Unity editor
    public float shuffleDuration = 1f; // Duration of the shuffle animation

    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private GameObject betterLuckPanel;


    private bool isShuffling = false;

    private void Start()
    {
        winPanel.SetActive(false);
        betterLuckPanel.SetActive(false);
        ShuffleCards();
    }

    public void OnCardClick(int cardIndex)
    {
        print("OnCardClick");
        if (!isShuffling) // Ensure the game is not shuffling before checking the result
        {
            if (cards[cardIndex].name == "Joker") // Assuming the name of the joker button is "Joker"
            {
                Debug.Log("You win!");
                float balance = PlayerPrefs.GetFloat("Balance");

                balance += PlayerStats.Instance.BetAmount * 2;

                PlayerPrefs.SetFloat("Balance", balance);

                StartCoroutine(ShowWin());

                pointsText.text = (PlayerStats.Instance.BetAmount * 2) + "";
            }
            else
            {
                Debug.Log("You lose!");

                StartCoroutine(ShowBetterLuck());
            }

            // Activate child objects of all cards
            foreach (GameObject card in cards)
            {
                ActivateChildObjects(card);
            }
        }
    }

    IEnumerator ShowBetterLuck()
    {
        yield return new WaitForSeconds(3);
        betterLuckPanel.SetActive(true);
    }

    IEnumerator ShowWin()
    {
        yield return new WaitForSeconds(3f);

        winPanel.SetActive(true);
    }

    private void ActivateChildObjects(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShuffleAnimationCoroutine()
    {
        Vector3[] initialPositions = new Vector3[cards.Length];

        // Store initial positions
        for (int i = 0; i < cards.Length; i++)
        {
            initialPositions[i] = cards[i].transform.position;
        }

        float timer = 0f;

        while (timer < shuffleDuration)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                // Randomize the position of the cards
                cards[i].transform.position = Vector3.Lerp(initialPositions[i], Random.insideUnitSphere * 3f, timer / shuffleDuration);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset positions
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].transform.position = initialPositions[i];
        }

        // Interchange positions
        for (int i = 0; i < cards.Length; i++)
        {
            int randomIndex = Random.Range(i, cards.Length);
            Vector3 tempPosition = cards[i].transform.position;
            cards[i].transform.position = cards[randomIndex].transform.position;
            cards[randomIndex].transform.position = tempPosition;
        }

        isShuffling = false;
    }

    private void ShuffleCards()
    {
        if (!isShuffling)
        {
            isShuffling = true;
            StartCoroutine(ShuffleAnimationCoroutine());
        }
    }
}
