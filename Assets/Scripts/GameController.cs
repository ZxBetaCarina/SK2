using SK2;
using System;
using System.Collections;
using System.Collections.Generic;
using RainbowJump.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private Manager manager; 
    public GameObject targetObject;  // Reference to the GameObject to be disabled/enabled
    public Button disableButton;     // Reference to the UI button for disabling
    public TMP_Text timerText;           // Reference to the UI text for displaying the timer
    public GameObject revealFX;
    [SerializeField] private GameObject _winningFX;
    private bool isButtonDisabled = false;
    private float disableDuration = 60f;
    private float timer;
    [SerializeField] private TextMeshProUGUI _bettingInput;
    // [SerializeField] private TextMeshProUGUI CoolTimer;

    [SerializeField] public TMP_Text _grand_prize_text;
    [SerializeField] public  TMP_Text _major_prize_text;
    [SerializeField] public TMP_Text _minor_prize_text;

    [SerializeField]public float _grand_prize_value;
    [SerializeField]public float _major_prize_value;
    [SerializeField]public float _minor_prize_value;

    [SerializeField] float _grand_prize_initial_value;
    [SerializeField] float _major_prize_initial_value;
    [SerializeField] float _minor_prize_initial_value;


    [SerializeField] private TemporaryDataStorage _temporaryDataStorage;
    public Button _increase_bet_button;
    public Button _decrease_bet_button;

    [SerializeField] private Button _maxBet;
    [SerializeField] private Button _minBet;

    [SerializeField] private TextMeshProUGUI _totalBet;
    public TextMeshProUGUI _currentPointsText;
    [SerializeField] private GameObject coinImages;
    [SerializeField] private GameObject _coinFx;

    public List<Vector3> _patterns;

    [SerializeField] private Button NormalPaytable;
    [SerializeField] private Button FollowPaytable;
    [SerializeField] private GameObject _pattern_FX;
    [SerializeField] private float _normalWinningMultiple;
    [SerializeField] private float _rubicModeWinningMultiple;
    [SerializeField] private float _jackPotModeWinningMultiple;
    [SerializeField] private AudioSource _winningAudio;
    [SerializeField] private GameObject _jackpotModepanel;
    
    [SerializeField] private Sprite bonus1;
    [SerializeField] private Sprite bonus2;
    [field: SerializeField] public Button RestartLevel { get; private set; }

    public bool Is_Balance_Sufficient
    {
        get
        {
            if (PlayerPrefs.GetFloat("Balance") >= _betPoints[_current_bet_index])
            {
                return true;
            }
            else
            {
                int temp = _current_bet_index - 1;
                while (temp >= 0)
                {
                    if (PlayerPrefs.GetFloat("Balance") >= _betPoints[temp])
                    {
                        print(temp);
                        _current_bet_index = temp;
                        _bettingInput.text = _bet_intervals_in_usd[_current_bet_index] + "USD";
                        _totalBet.text = _betPoints[_current_bet_index] + "Pts";
                        return true;
                    }
                    temp--;
                }
                return false;
            }
        }
    }

    public List<float> _bet_intervals_in_usd;
    public string _playerprefs_balance_key;
    private readonly float[] _betPoints = { 0.1f, 0.5f, 1f, 1.5f, 2f, 2.5f };
    private int _initialBet;
    public int _current_bet_index { get; private set; }

    public float _currentPoints = 1000f;                // Starting points
    public int _point_multiplier = 100;

    public int _display_points;

    [SerializeField] private TMP_Text _availableCredits;
    public static Action BetChanged;
    public static Action BetConfirmed;
    public bool JackPotMode;

    public void AddPatternPositions(Vector3 position)
    {
        _patterns.Add(position);
    }
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RestartLevel.onClick.AddListener(() =>
        {
            // PlayerPrefs.SetFloat("Balance", _currentPoints);  
            // PlayerPrefs.Save();
           // PlayerPrefs.SetInt("CurrentBetIndex", _current_bet_index); 
           // PlayerPrefs.SetFloat("CurrentBetAmountUSD", _bet_intervals_in_usd[_current_bet_index]);  
           // PlayerPrefs.Save();  // Make sure the changes are saved to disk
           // Debug.Log("CurrentBetIndex"+PlayerPrefs.GetFloat("CurrentBetIndex")); 
            SceneManager.LoadScene(1); 
            
        });
        //Debug.Log("CurrentBetIndex"+PlayerPrefs.HasKey("CurrentBetIndex"));
        _current_bet_index = PlayerStats.Instance.CurrentBetIndex;
    }

    private void OnEnable()
    {
        CheckForWinningPatterns.PatternFound += OnPatternFound;
    }

    public void OnDisable()
    {
        CheckForWinningPatterns.PatternFound -= OnPatternFound;
    }

    void Start()
    {
        //QualitySettings.vSyncCount = 50;
        //Application.targetFrameRate = 50;

        PlayerStats.Instance.GameStartedOnce();
        JackPotMode = false;
        _jackpotModepanel.SetActive(false);
        disableButton.onClick.AddListener(DisableObject);
        InitiateBet();
        AvailableCredit();

        //PlayerPrefs.SetFloat("Balance", _currentPoints);   // ENABLE THIS TO RESET THE PLAYERPREFS WITH DEFAULT VALUE

        if (PlayerPrefs.HasKey("Balance"))
        {
            _currentPoints = PlayerPrefs.GetFloat("Balance");
           
            _display_points = (int)Mathf.Round(_currentPoints * _point_multiplier);
            //print(_currentPoints);
            print("Update Points 1");
            _currentPointsText.text = _display_points + "";
        }
        
        if (_current_bet_index == 0)
        {
            _decrease_bet_button.interactable = false;
            _increase_bet_button.interactable = true;
        }
        else if (_current_bet_index == 5)
        {
            _decrease_bet_button.interactable = true;
            _increase_bet_button.interactable = false;
        }
        else
        {
            _decrease_bet_button.interactable = true;
            _increase_bet_button.interactable = true;
        }
        _current_bet_index = PlayerStats.Instance.CurrentBetIndex;
    }

    void Update()
    {
        if (isButtonDisabled)
        {
            timer -= Time.deltaTime;

            // Update timer text
            timerText.text = "Cooldown: " + Mathf.Ceil(timer).ToString();

            if (timer <= 0f)
            {
                // Enable the button after the cooldown
                isButtonDisabled = false;
                disableButton.interactable = true;
                timerText.text = "";
            }
        }
        
    }

    void AvailableCredit()
    {
        int roundedValue = Mathf.FloorToInt(_currentPoints);

        // Display the rounded value
        if (_availableCredits != null)
        {
            _availableCredits.text = roundedValue.ToString() + "USD";
        }
        else
        {
            Debug.LogWarning("Display Text reference not set.");
        }

        // Alternatively, you can simply print it out
        Debug.Log("Nearest Small Whole Number:  " + roundedValue);
    }
    void DisableObject()
    {
        // Disable the target object
        targetObject.SetActive(false);
        revealFX.SetActive(true);
        // Enable it back after 3 seconds
        StartCoroutine(EnableObjectAfterDelay(3f));

        // Disable the button for the cooldown duration
        disableButton.interactable = false;
        isButtonDisabled = true;
        timer = disableDuration;

        // Start the countdown timer
        StartCoroutine(CountdownTimer());
    }

    IEnumerator EnableObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(true);
        revealFX.SetActive(false);
        PatternDetector.INSTANCE.DisableLastRow();
    }

    IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            yield return null;
        }
    }

    private void OnPatternFound()
    {
        _winningAudio.Play();
        _coinFx.SetActive(true);
        ImageRNGSpawner.Instance.won = true;
        foreach (var pos in _patterns)
        {
            GameObject fx = Instantiate(_pattern_FX, pos, Quaternion.identity);
            Destroy(fx, 3f);
        }

        if (UIManager.Instance.RubicMode)
        {
            //float winningAmount = _rubicModeWinningMultiple * _betPoints[_current_bet_index] / 100;
            float winningAmount = _betPoints[_current_bet_index];
            _currentPoints += winningAmount;

            _display_points = (int)_currentPoints * _point_multiplier;
        }
        else if (ImageCylinderSpawner.INSTANCE.currentSpinJackpot != JackpotTypes.None)
        {
            float winningAmount = 0;
            switch (ImageCylinderSpawner.INSTANCE.currentSpinJackpot)
            {
                case JackpotTypes.Minor:
                    winningAmount = _minor_prize_value / _point_multiplier;
                    print(winningAmount);
                    break;

                case JackpotTypes.Major:
                    winningAmount = _major_prize_value / _point_multiplier;
                    print(winningAmount);
                    break;

                case JackpotTypes.Grand:
                    winningAmount = _grand_prize_value / _point_multiplier;
                    print(winningAmount);
                    break;
            }
            _currentPoints += winningAmount;

            _display_points = (int)_currentPoints * _point_multiplier;
        }
        else if (JackPotMode)
        {
            //float winningAmount = _jackPotModeWinningMultiple * _betPoints[_current_bet_index] / 100;
            float winningAmount =_betPoints[_current_bet_index];
            _currentPoints += winningAmount;

            _display_points = (int)_currentPoints * _point_multiplier;

            JackPotMode = false;
        }
        else
        {
            //float winningAmount = _normalWinningMultiple * _betPoints[_current_bet_index] / 100;
            float winningAmount = _betPoints[_current_bet_index];
            _currentPoints += winningAmount;

            _display_points = (int)Mathf.Round(_currentPoints * _point_multiplier);

        }
        print(_currentPoints);
        print("Update Points 2");
        _currentPointsText.text = _display_points + "";
        PlayerPrefs.SetFloat("Balance", _currentPoints);
        print("<color=green>PATTERN FOUND</color>");
        AvailableCredit();
        Invoke(nameof(DisableCoinFx), 3f);
        GameObject winningFx = Instantiate(_winningFX);
        Destroy(winningFx, 4f);

        _patterns = new();
        if (CheckForWinningPatterns.INSTANCE.WinningIconName == bonus1)
        {
            RestartLevel.gameObject.SetActive(false);
        }
        else if (CheckForWinningPatterns.INSTANCE.WinningIconName == bonus2)
        {
            
        }
        else
        {
            RestartLevel.gameObject.SetActive(false);
        }

    }

    public void FinaliseBetOnClickSpin()
    {
        /*if (_currentPoints >= _currentPoints - _betPoints[_current_bet_index])
        {
            if (_currentPoints - _betPoints[_current_bet_index] < 0) return;
            _currentPoints -= _betPoints[_current_bet_index];
            _currentPointsText.text = _currentPoints + "Pts";
            PlayerStats.Instance.CurrentBetIndex = _current_bet_index;
            AvailableCredit();
            _increase_bet_button.interactable = false;
            _decrease_bet_button.interactable = false;
            _maxBet.interactable = false;
            _minBet.interactable = false;
            CheckForWinningPatterns.INSTANCE.FinaliseBet();

            BetConfirmed?.Invoke();

            int betAmount = (int)_betPoints[_current_bet_index] / 100;

            PlayerStats.Instance.SetBetAmount(betAmount);
            if (_current_bet_index == 5)
            {
                JackPotMode = true;
                StartJackPotMode();
            }
            else
            {
                JackPotMode = false;
            }
        }*/
        if (_currentPoints > _bet_intervals_in_usd[_current_bet_index])
        {
            _currentPoints -= _bet_intervals_in_usd[_current_bet_index];

            _display_points = (int)Mathf.Round(_currentPoints * _point_multiplier);

            //print("<color=green>PATTERN FOUND</color>");
            print(_currentPoints);
            print("Update Points 3");
            _currentPointsText.text = _display_points + "";
        }
        else
        {
            print("INSUFFICIENT BALANCE");
        }
    }

    public void OnClickReviewButton()
    {
        CheckForWinningPatterns.INSTANCE.ReviewImages(true);
    }

    public void StartJackPotMode()
    {
        _jackpotModepanel.SetActive(true);
    }

    public void JackPotWinning()
    {
        Debug.Log("JackPot!!!");

    }

    public void EndJackPotMode()
    {
        JackPotMode = false;
        _jackpotModepanel.SetActive(false);
    }


    private void DisableCoinFx()
    {
        _coinFx.SetActive(false);
    }

    public void Button_Increase_Bet()
    {
        if (!ImageCylinderSpawner.INSTANCE.CylinderSpawning)
        {
            /*if (CurrentBetIndex < 5 && CurrentBetIndex >= 0)
            {
                if (!Is_Balance_Sufficient)
                {
                    UIManager.Instance.TriggerBalanceInsufficient();
                    return;
                }

                CurrentBetIndex++;
                Mathf.Clamp(CurrentBetIndex, 0, 4);
                _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "USD";
                _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
                _temporaryDataStorage.SetCurrentBet = _betPoints[CurrentBetIndex];
                PlayerStats.Instance.CurrentBetIndex = CurrentBetIndex;

                CheckForWinningPatterns.INSTANCE.ReviewImages(false);
                BetChanged?.Invoke();
                if (CurrentBetIndex == 0)
                {
                    _minBet.interactable = false;
                    _maxBet.interactable = true;
                }
                else if (CurrentBetIndex == 5)
                {
                    _maxBet.interactable = false;
                    _minBet.interactable = true;
                }
                else
                {
                    _maxBet.interactable = true;
                    _minBet.interactable = true;
                }
            }*/

            ++_current_bet_index;
            _grand_prize_value =_grand_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
            _grand_prize_text.text = _grand_prize_value.ToString();
            PlayerPrefs.SetFloat("_grand_prize_value", _grand_prize_value);

            _major_prize_value = _major_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
            _major_prize_text.text = _major_prize_value.ToString();
            PlayerPrefs.SetFloat("_major_prize_value", _major_prize_value);


            _minor_prize_value = _minor_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
            _minor_prize_text.text = _minor_prize_value.ToString();
            PlayerPrefs.SetFloat("_minor_prize_value", _minor_prize_value);
            PlayerPrefs.Save();


            if (_current_bet_index + 1 >= _bet_intervals_in_usd.Count ||
                PlayerPrefs.GetFloat(_playerprefs_balance_key) < _bet_intervals_in_usd[_current_bet_index + 1])
            {
                _increase_bet_button.interactable = false;
            }

            _bettingInput.text = _bet_intervals_in_usd[_current_bet_index].ToString("F2") + " USD";
            PlayerStats.Instance.CurrentBetIndex = _current_bet_index;

            if (_current_bet_index > 0)
            {
                _decrease_bet_button.interactable = true;
            }

            ImageCylinderSpawner.INSTANCE.RefreshCylinder();

            
        }
    }

    public void Button_Decrease_Bet()
    {
        --_current_bet_index;
        _grand_prize_value = _grand_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
        PlayerPrefs.SetFloat("_grand_prize_value", _grand_prize_value);
        _grand_prize_text.text = _grand_prize_value.ToString();
        

        _major_prize_value = _major_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
        PlayerPrefs.SetFloat("_major_prize_value", _major_prize_value);
        _major_prize_text.text = _major_prize_value.ToString();
        
        _minor_prize_value = _minor_prize_initial_value * (_current_bet_index == 0 ? 1 : (5 * _current_bet_index));
        PlayerPrefs.SetFloat("_minor_prize_value", _minor_prize_value);
        _minor_prize_text.text = _minor_prize_value.ToString();
        PlayerPrefs.Save(); 

        if (!ImageCylinderSpawner.INSTANCE.CylinderSpawning)
        {
            if (_current_bet_index - 1 < 0)
            {
                _decrease_bet_button.interactable = false;
            }

            _bettingInput.text = _bet_intervals_in_usd[_current_bet_index].ToString("F2") + " USD";
            PlayerStats.Instance.CurrentBetIndex = _current_bet_index;

            if (!_increase_bet_button.interactable)
            {
                _increase_bet_button.interactable = true;
            }
            ImageCylinderSpawner.INSTANCE.RefreshCylinder();
        }

        
    }

    /*public void DecreaseBet()
    {
        if (!ImageCylinderSpawner.INSTANCE.CylinderSpawning)
        {
            if (_current_bet_index <= 5 && _current_bet_index > 0)
            {
                print(_current_bet_index);
                if (!Is_Balance_Sufficient)
                {
                    UIManager.Instance.TriggerBalanceInsufficient();
                    return;
                }
                print(_current_bet_index);
                //CurrentBetIndex--;
                Mathf.Clamp(_current_bet_index, 0, 4);
                print(_current_bet_index);
                _bettingInput.text = _bet_intervals_in_usd[_current_bet_index] + "USD";
                _totalBet.text = _betPoints[_current_bet_index] + "Pts";
                _temporaryDataStorage.SetCurrentBet = _betPoints[_current_bet_index];
                PlayerStats.Instance.CurrentBetIndex = _current_bet_index;
                CheckForWinningPatterns.INSTANCE.ReviewImages(false);
                BetChanged?.Invoke();
            }
            if (_current_bet_index == 0)
            {
                _minBet.interactable = false;
                _maxBet.interactable = true;
            }
            else if (_current_bet_index == 5)
            {
                _maxBet.interactable = false;
                _minBet.interactable = true;
            }
            else
            {
                _maxBet.interactable = true;
                _minBet.interactable = true;
            }
        }
    }*/
    

    public void InitiateBet()
    {
        AvailableCredit();
        if (PlayerPrefs.HasKey("_grand_prize_value"))
        {
            _grand_prize_value = PlayerPrefs.GetFloat("_grand_prize_value");
            _grand_prize_text.text = _grand_prize_value.ToString();
        }
        else
        {
            // Fallback to initial values if no saved data exists
            _grand_prize_value = _grand_prize_initial_value;
        }

        if (PlayerPrefs.HasKey("_major_prize_value"))
        {
            _major_prize_value = PlayerPrefs.GetFloat("_major_prize_value");
            _major_prize_text.text = _major_prize_value.ToString();
        }
        else
        {
            // Fallback to initial values if no saved data exists
            _major_prize_value = _major_prize_initial_value;
        }

        if (PlayerPrefs.HasKey("_minor_prize_value"))
        {
            _minor_prize_value = PlayerPrefs.GetFloat("_minor_prize_value");
            _minor_prize_text.text = _minor_prize_value.ToString();
        }
        else
        {
            // Fallback to initial values if no saved data exists
            _minor_prize_value = _minor_prize_initial_value;
            _grand_prize_value = _major_prize_initial_value;
            _major_prize_value = _major_prize_initial_value;    
        }

        //print(PlayerStats.Instance.CurrentBetIndex);
        _current_bet_index = PlayerStats.Instance.CurrentBetIndex;
       // PlayerPrefs.SetInt("CurrentBetIndex", _current_bet_index);
       // PlayerPrefs.SetFloat("CurrentBetAmountUSD", _bet_intervals_in_usd[_current_bet_index]);
        bool test = Is_Balance_Sufficient;
        //_increase_bet_button.interactable = true;
        //_decrease_bet_button.interactable = true;
        _maxBet.interactable = true;
        _bettingInput.text = _bet_intervals_in_usd[_current_bet_index] + "USD";
        _totalBet.text = _betPoints[_current_bet_index] + "Pts";

        float temp = PlayerPrefs.GetFloat("Balance");

        _display_points = (int)Mathf.Round(temp * _point_multiplier);

        print(_display_points);
        print("Update Points 4");
        _currentPointsText.text = _display_points + "";
        //NormalPaytable.gameObject.SetActive(true);
        //FollowPaytable.gameObject.SetActive(false);
        timer = 0;
        //timerText.text = " ";

    }
    
    void OnApplicationQuit()
    {
        
        PlayerPrefs.DeleteKey("_grand_prize_value");
        PlayerPrefs.DeleteKey("_major_prize_value");
        PlayerPrefs.DeleteKey("_minor_prize_value");
        
        PlayerPrefs.Save();
    }
}
