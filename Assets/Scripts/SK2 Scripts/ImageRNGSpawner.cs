using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SK2
{
    public class ImageRNGSpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask _imageLayer;

        [Header("Slot Objects")]
        [SerializeField] private GameObject[] imagePrefabs;  // Array to hold different sets of image prefabs
        [SerializeField] private Transform[] _imagePosition;
        [SerializeField] private Transform[] _doors;

        [Header("Processing")]
        public AudioSource slotAudio;
        private GameObject allCylindersParent;  // New parent for all cylinders

        //public Slider speedSlider;  // Reference to the UI slider for speed adjustment
        //public TMP_Text speedText;  // Reference to the UI text for displaying speed
        //public TMP_Text coinsText;  // Reference to the UI text for displaying coins
        [SerializeField] private float speed;
        private int coins = 100;  // Initial coins
        private bool hasMoney = true;
        //public Button muteButton;  // Reference to the UI button for muting/unmuting
        public bool isMuted = false;  // Flag to track whether the game is currently muted
        public GameObject fxPrefab;
        public GameObject spinButtonDup;
        [SerializeField] Button _spinButton;
        public string nextSceneName = "MainMenu";
        public int _difficultyFactor = 0;
        public bool CylinderSpawning = false;
        private ImageCylinderSpawner ics; 
        public GameObject popupPanel;  // Assign the pop-up panel in the Inspector
                                       // public TMP_Text popupText;  // Assign the Text element for the message
        public Button closeButton;  // Assign the Button element for closing the pop-up
        private bool checkedForPatterns = false;
        //public Button refreshBtn;
        public static global::SK2.ImageRNGSpawner Instance;        //Singleton Instance
        private bool _betConfirmed = false;
        public bool won = false;

        private bool bonusSpins = false;

        private bool imagesRefreshed = false;
        private GameObject[,] _imageSpawner;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            GameController.BetConfirmed += OnBetConfirmed;
            GameController.BetChanged += OnBetChanged;
        }
        private void OnDisable()
        {
            GameController.BetConfirmed -= OnBetConfirmed;
            GameController.BetChanged -= OnBetChanged;
        }
        void Start()
        {
            ics = GetComponent<ImageCylinderSpawner>(); 
            allCylindersParent = new GameObject("AllCylindersParent");  // Create a new empty GameObject as the parent
            GenerateTiles();

            // Add a listener to the slider
            //speedSlider.onValueChanged.AddListener(ChangeRotationSpeed);
            // Add a listener to the mute button
            //muteButton.onClick.AddListener(ToggleMute);

            // Display initial coins
            UpdateCoinsText();

            // Ensure the pop-up panel is initially hidden
            popupPanel.SetActive(false);

            // Add a listener to the close button
            closeButton.onClick.AddListener(HidePopup);
        }

        private void OnBetChanged()
        {
            _difficultyFactor = GameController.Instance._current_bet_index;
            GenerateTiles();
        }

        public void SpinSlot()
        {
            GameController.Instance.FinaliseBetOnClickSpin();
            if (!_betConfirmed) return;
            if (hasMoney == true && coins > 0)
            {
                StartCoroutine(SpinProcedure());
            }
            // Check if the player has enough coins
            if (coins >= 20)
            {
                // Deduct coins
                UpdateCoinsText();  // Update UI text
                if (fxPrefab != null)
                {
                    // Instantiate the FX Prefab at the button's position
                    GameObject fxInstance = Instantiate(fxPrefab, spinButtonDup.transform.position, Quaternion.identity);
                    slotAudio.Play();
                    // Optionally, you can destroy the fxInstance after a certain duration
                    Destroy(fxInstance, 2f); // Adjust the duration as needed
                }
            }
            else
            {
                // Display not enough money message
                Debug.Log("Not enough money!");
            }
        }

        private IEnumerator SpinProcedure()
        {
            _spinButton.interactable = false;
            yield return StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
            
                StartCoroutine(PatternDetector.INSTANCE.CheckForPatterns());

            
            
            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
            GameController.Instance.RestartLevel.gameObject.SetActive(true);
            //Check Pattern

            if (checkedForPatterns)
            {
                checkedForPatterns = false;
            }
        }

        private void GenerateTiles()
        {
            //Generate Tile
            for (int i = 0; i < 12; i++)
            {
                GameObject tile = Instantiate(imagePrefabs[UnityEngine.Random.Range(0, imagePrefabs.Length)]);
                tile.name = $"Tile _{tile.GetComponent<Icons>().GetSprite().name}";
                tile.transform.position = _imagePosition[i].position;
            }
        }

        private void TestingDetector()
        {

            //Generate Tile
            for (int i = 0; i < 12; i++)
            {
                GameObject tile;
                if (i == 0 || i == 1 || i == 2)
                    tile = Instantiate(imagePrefabs[0]);
                else
                    tile = Instantiate(imagePrefabs[UnityEngine.Random.Range(0, imagePrefabs.Length)]);
                tile.name = $"Tile _{tile.GetComponent<Icons>().GetSprite().name}";
                tile.transform.position = _imagePosition[i].position;
            }
        }

        public void OnBetConfirmed()
        {
            _difficultyFactor = GameController.Instance._current_bet_index;
            _betConfirmed = true;
        }

        IEnumerator RotateSymbolTowardsCenter(Transform symbol, Quaternion targetRotation, float duration)
        {
            float elapsed = 0f;
            Quaternion startRotation = symbol.rotation;

            while (elapsed < duration)
            {
                symbol.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure the symbol is perfectly aligned
            symbol.rotation = targetRotation;
        }

        void UpdateCoinsText()
        {
            // Update the coins text
            //coinsText.text = $"Coins: {coins}";
        }
        //void ToggleMute()
        //{
        //    isMuted = !isMuted;  // Toggle the mute state

        //    // Adjust the volume based on the mute state
        //    AudioListener.volume = isMuted ? 0.1f : 1.0f;

        //    // Update the button text
        //    muteButton.GetComponentInChildren<Text>().text = isMuted ? "Unmute" : "Mute";
        //}
        public void ChangeScene()
        {
            SceneManager.LoadScene(nextSceneName);
        }

        public void ShowPopup(string message)
        {
            //  popupText.text = message;
            popupPanel.SetActive(true);
        }

        public void HidePopup()
        {
            popupPanel.SetActive(false);
        }

        //public void RefreshImages()
        //{
        //    if (!isRotating)
        //    {
        //        // Destroy existing image symbols
        //        if (!CylinderSpawning)
        //        {
        //            refreshBtn.interactable = false;
        //            CylinderSpawning = true;
        //            StartCoroutine(nameof(RefreshImagesFX));
        //        }
        //    }
        //}

        //public void RefreshImagesAgain()
        //{
        //    if (!isRotating)
        //    {
        //        // Destroy existing image symbols
        //        if (!CylinderSpawning)
        //        {
        //            CylinderSpawning = true;
        //            StartCoroutine(nameof(RefreshImagesFX));
        //            refreshBtn.interactable = true;

        //        }
        //    }
        //}

        //private IEnumerator RefreshImagesFX()
        //{
        //    //destroy previous images fx
        //    foreach (var parent in cylinderParents)
        //    {
        //        foreach (Transform child in parent)
        //        {
        //            if (!child.gameObject.activeInHierarchy)
        //            {
        //                child.gameObject.SetActive(true);
        //            }
        //            child.gameObject.AddComponent<Rigidbody2D>();
        //            Destroy(child.gameObject, 2f);
        //        }
        //        Destroy(parent.gameObject, 2f);
        //    }
        //    yield return new WaitForSeconds(2);
        //    //create new images fx
        //    // Spawn new image symbols inside allCylindersParent
        //    Vector3 spawnPosition = Vector3.zero;
        //    for (int i = 0; i < numberOfCylinders; i++)
        //    {
        //        SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform);
        //        spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        //    }
        //    CylinderSpawning = false;

        //}

        private void CheckWinningCondition()
        {
            if (CheckForWinningPatterns.INSTANCE != null)
            {
                CheckForWinningPatterns.INSTANCE.CheckPatterns();
                checkedForPatterns = true;
            }
        }

        //public void StartBonusSpin()
        //{
        //    StartCoroutine(FifteenBonusSpins());
        //}


        public IEnumerator FifteenBonusSpins()
        {
            //refreshBtn.interactable = false;
            bonusSpins = true;

            //yield return StartCoroutine(RefreshImagesFX());

            SpinSlot();

            //yield return StartCoroutine(StopCylindersSequentially());

            yield return null;
            bonusSpins = false;
        }
    }




}
