using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Text scoreText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text pointsEarnedText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject backToHomeBtn;

    private int score;
    public int Score => score;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            //Application.targetFrameRate = 60;
            DontDestroyOnLoad(gameObject);
            Pause();
        }
        gameOver.SetActive(false);
        pointsEarnedText.gameObject.SetActive(false);
        pointsText.gameObject.SetActive(false);
        backToHomeBtn.SetActive(false);
    }

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void GameOver()
    {
        //playButton.SetActive(true);
        gameOver.SetActive(true);

        pointsText.text = (score * .01f) + "$";

        float points = PlayerPrefs.GetFloat("Balance");
        points = points + score * .01f;

        PlayerPrefs.SetFloat("Balance", points);

        pointsEarnedText.gameObject.SetActive(true);
        pointsText.gameObject.SetActive(true);
        backToHomeBtn.SetActive(true);


        Pause();
    }


    public void LoadLevel1()
    {
        Debug.Log("CurrentBetIndex 5 "+PlayerPrefs.GetFloat("CurrentBetIndex")); 
        SceneManager.LoadScene("Level 1");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

}
