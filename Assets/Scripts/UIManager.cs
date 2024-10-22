using SK2;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField, Tooltip("Panel shown if available balance insuffiecient")] private GameObject balanceInsuffucientPanel;
    [SerializeField] private float balanceInsuffucientPanelActiveForSec;

    [SerializeField] private TextMeshProUGUI handsPlayed;

    [SerializeField] private GameObject _winningPanel;
    [SerializeField] private TextMeshProUGUI _winningText;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private TextMeshProUGUI _waitingText;
    [SerializeField] private List<GameObject> RubicControllers;
    [SerializeField] private GameObject _revealbutton;
    // [SerializeField] private GameObject _muteButton;
    [SerializeField] private GameObject _followPanel;
    [SerializeField] private Button _spinButton;
    [SerializeField] private Button NormalPaytable;
    [SerializeField] private Button FollowPaytable;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private GameObject _showRubicButton;
    [SerializeField] private GameObject freeSpinImage;
    private string _waitPrefix = "Please Wait... ";
    private string _playAgainString = "Collect";
    private string _winningMsg = "Got It!!";
    private string _losignMsg = "You Lose";
    [SerializeField] private float _winningPanelDelay = 5f;
    public bool RubicMode { get; private set; }
    private int _movesLeft;

    [SerializeField] private ReadCube _readCube;
    [SerializeField] private CubeState _cubeState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        handsPlayed.text = PlayerStats.Instance.HandsPlayed.ToString();
    }

    public void HandsPlayedIncrement(string count)
    {
        handsPlayed.text = count;
    }

    private void OnEnable()
    {
        GameController.BetConfirmed += OnBetConfirmed;
        FollowPanelTimer.TimerEnded += OnFollowTimerEnded;
        CheckForWinningPatterns.PatternNotFound += OnPatternNotFound;
        CheckForWinningPatterns.PatternFound += OnPatternFound;
    }

    private void OnDisable()
    {
        GameController.BetConfirmed -= OnBetConfirmed;
        FollowPanelTimer.TimerEnded -= OnFollowTimerEnded;
        CheckForWinningPatterns.PatternNotFound -= OnPatternNotFound;
        CheckForWinningPatterns.PatternFound -= OnPatternFound;
    }

    IEnumerator Start()
    {
        _readCube = FindObjectOfType<ReadCube>();
        _cubeState = FindObjectOfType<CubeState>();

        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(false);
        }
        _revealbutton.SetActive(false);
        _followPanel.SetActive(false);
        _showRubicButton.SetActive(false);
        _spinButton.gameObject.SetActive(true);
        StartCoroutine(nameof(DelaySpinButton));
        _refreshButton.interactable = true;
        RubicMode = false;
        _movesLeft = 10;
        yield return null;
    }

    private void OnFollowTimerEnded()
    {
        //StartCoroutine(nameof(SetWinningPanelActive), _losignMsg);
    }
    private void OnBetConfirmed()
    {
        _refreshButton.interactable = false;
        //_readCube.ReadState();
    }

    public void StartGameSetUp()
    {
        SceneManager.LoadScene(1);
    }

    public void TriggerBalanceInsufficient()
    {
        StartCoroutine(BalanceInsufficientFlow());
    }

    private IEnumerator BalanceInsufficientFlow()
    {
        balanceInsuffucientPanel.SetActive(true);
        yield return new WaitForSeconds(balanceInsuffucientPanelActiveForSec);
        balanceInsuffucientPanel.SetActive(false);
    }

    private IEnumerator DelaySpinButton()
    {
        yield return new WaitForSeconds(2);
        //_spinButton.interactable = true;
    }

    public void CloseRubicMode()
    {
        RubicMode = false;
        _revealbutton.SetActive(false);
        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(false);
        }
    }

    private void OnPatternNotFound()
    {
        GameController.Instance.EndJackPotMode();
        if (RubicMode && _movesLeft > 0)
        {
            _movesLeft--;
            return;
        }
        else if (RubicMode && _movesLeft == 0)
        {
            CloseRubicMode();
            _winningPanel.SetActive(true);
            _winningText.text = _losignMsg;
            _waitingText.text = "Play Again";
            _message.text = "";
            PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);
            return;
        }
        PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);
        RubicMode = false;
        _winningPanelDelay = 0f;
        //_spinButton.interactable = false;

        if (ImageCylinderSpawner.INSTANCE.IsLastChanceToWin)
        {
            StartCoroutine(PlayAgain());
        }
        else if (!ImageCylinderSpawner.INSTANCE.CanShiftCylinder)
        {
            ImageCylinderSpawner.INSTANCE.CanShiftCylinder = true;
        }
        //_followPanel.SetActive(true);

    }

    private IEnumerator PlayAgain()
    {
        if (GameController.Instance.RestartLevel.gameObject.activeInHierarchy)
        {
            yield break;
        }
        while (DoorAnim.INSTANCE.IsAnimRunning) yield return null;

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
        GameController.Instance.RestartLevel.gameObject.SetActive(true);
    }


    public void ShowFreeSpin()
    {
        StartCoroutine(FreeSpinShow());
    }


    IEnumerator FreeSpinShow()
    {
        freeSpinImage.SetActive(true);
        yield return new WaitForSeconds(2f);
        freeSpinImage.SetActive(false);
    }

    //private IEnumerator WaitForUserInput()
    //{
    //    yield return new WaitForSeconds(30f);
    //    _followPanel.SetActive(false);
    //    StartCoroutine(nameof(SetWinningPanelActive), _losignMsg);
    //}
    public void TurnOnHelperButtons()
    {
        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(true);
        }
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        _revealbutton.SetActive(true);
    }

    private void OnPatternFound()
    {
        GameController.Instance.EndJackPotMode();
        CloseRubicMode();
        //Highlight matchingPatterns
        _winningPanelDelay = 3f;
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        RubicMode = false;
        if (!CheckForWinningPatterns.INSTANCE.isBonus)
        {
            StartCoroutine(nameof(SetWinningPanelActive), _winningMsg);
        }
        //StopCoroutine(WaitForUserInput());
    }

    private IEnumerator SetWinningPanelActive(string text)
    {
        _playAgainButton.interactable = false;
        yield return new WaitForSeconds(_winningPanelDelay);
        //  int _waitTimer = 15;
        if (ImageCylinderSpawner.INSTANCE.currentSpinJackpot != JackpotTypes.None && ImageCylinderSpawner.INSTANCE.currentSpinJackpot != JackpotTypes.FreeSpin)
        {
            switch (ImageCylinderSpawner.INSTANCE.currentSpinJackpot)
            {
                case JackpotTypes.Minor:
                    _winningText.text = "Minor Jackpot";
                    break;

                case JackpotTypes.Major:
                    _winningText.text = "Major Jackpot";
                    break;

                case JackpotTypes.Grand:
                    _winningText.text = "Grand Jackpot";
                    break;
            }
            print("JACKPOT HIT");
            print("Winning Panel");
            _winningPanel.SetActive(true);
        }
        // _winningText.text = text;
        //  _message.text = "";
        //while (_waitTimer > 0)
        //{
        //    _waitingText.text = _waitPrefix + _waitTimer + "s";
        //    _waitTimer--;
        //    yield return new WaitForSeconds(1f);
        //}
        _waitingText.text = _playAgainString;
        _playAgainButton.interactable = true;
    }

    public void OnClickFollowButton()
    {
        StopAllCoroutines();
        RubicMode = true;
        TurnOnHelperButtons();
        _showRubicButton.SetActive(true);
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        NormalPaytable.gameObject.SetActive(false);
        FollowPaytable.gameObject.SetActive(true);
        _followPanel.SetActive(false);
    }


    public void RestartScene(int index)
    {
        SceneManager.LoadScene(index);
    }

}
