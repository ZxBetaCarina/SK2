using SK2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ImageCylinderSpawner : MonoBehaviour
{
    [SerializeField] private CheckForWinningPatterns checkForWinningPatterns;
    
    [Header("RNG")] [SerializeField] private WeightedRNG<GameObject> slotRNGItems;
    [SerializeField] private WeightedRNG<JackpotTypes> jackpotRNGItems;

    [SerializeField, Tooltip("These are the spots where jackpot occurs if we hit one")]
    private List<Transform> jackpotOccurPosition;

    [SerializeField, Tooltip("These are the spots where jackpot occurs if we hit one")]
    private List<Transform> jackpotOccurPositionBackup;

    [Header("Cylinder Data")] [SerializeField]
    private Vector3 _position;

    [SerializeField] private GameObject _shiftingPanel;

    [Header("Slot Image Cylinder")] [SerializeField]
    private LayerMask _imageLayer;

    public GameObject[] imagePrefabs; // Array to hold different sets of image prefabs
    public int numberOfImages = 21;
    public float cylinderRadius = 5f;
    public int numberOfCylinders = 3; // Update the number of cylinders
    public float rotationSpeed = 50f;
    public float CylinderStopInterval = 1f;
    public float distanceBetweenCylinders = 2f; // Distance between cylinders
    public float[] rotations;
    public bool _isRotating = false;
    public Transform[] cylinderParents;
    public Vector3[] cylinderSpawnPoints;
    public bool[] cylinderRotationStates;

    public Button _button_left;
    public Button _button_right;

    private Icons[] _spawnedIcons;
    [SerializeField] private float _swapSpeed;

    public Icons[] GetSpawnedIcons
    {
        get => _spawnedIcons;
    }

    public int _currentCylinder = 0;

    public AudioSource slotAudio;
    private GameObject allCylindersParent; // New parent for all cylinders

    public Slider speedSlider; // Reference to the UI slider for speed adjustment

    public TMP_Text speedText; // Reference to the UI text for displaying speed

    //public TMP_Text coinsText;  // Reference to the UI text for displaying coins
    public Button muteButton; // Reference to the UI button for muting/unmuting
    public bool isMuted = false; // Flag to track whether the game is currently muted
    public GameObject fxPrefab;
    public GameObject spinButtonDup;

    [SerializeField] Button _spinButton;

    public string nextSceneName = "MainMenu";
    public int _difficultyFactor = 0;
    public bool CylinderSpawning = false;

    public GameObject popupPanel; // Assign the pop-up panel in the Inspector

    // public TMP_Text popupText;  // Assign the Text element for the message
    public Button closeButton; // Assign the Button element for closing the pop-up
    private bool checkedForPatterns = false;
    public Button refreshBtn;
    public static ImageCylinderSpawner INSTANCE; //Singleton Instance
    private bool _betConfirmed = false;
    public bool won = false;

    private bool bonusSpins = false;

    public bool CanShiftCylinder = false;
    public bool IsLastChanceToWin = false;
    private bool imagesRefreshed = false;

    [SerializeField] private GameObject hideImage;
    [SerializeField] private GameObject hideImage1;
    [SerializeField] private GameObject hideImage2;
    [SerializeField] private GameObject hideImage3;
    [SerializeField] private GameObject hideImage4;
    [SerializeField] private GameObject hideImage5;

    public JackpotTypes currentSpinJackpot { get; private set; } = JackpotTypes.None;
    private Vector3 _cylinderInitialPos;

    [SerializeField] float _shift_duration;
    List<Vector3> _cylinder_positions = new List<Vector3>();

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }

        _spawnedIcons = new Icons[numberOfImages];
        _cylinderInitialPos = transform.localPosition;
    }

    private void OnEnable()
    {
        GameController.BetChanged += OnBetChanged;
        PatternDetector.PatternNotFound += OnPatternNotFound;
    }

    private void OnDisable()
    {
        GameController.BetChanged -= OnBetChanged;
        PatternDetector.PatternNotFound -= OnPatternNotFound;
    }

    void Start()
    {
        allCylindersParent = new GameObject("AllCylindersParent"); // Create a new empty GameObject as the parent

        cylinderParents = new Transform[numberOfCylinders];

        cylinderRotationStates = new bool[numberOfCylinders];
        SpawnCylinders();

        transform.position = _position;
        allCylindersParent.transform.localPosition = Vector3.zero;

        // Add a listener to the slider
        speedSlider.onValueChanged.AddListener(CylinderStopSpeed);
        // Add a listener to the mute button
        muteButton.onClick.AddListener(ToggleMute);

        // Display initial coins
        UpdateCoinsText();

        // Ensure the pop-up panel is initially hidden
        popupPanel.SetActive(false);

        // Add a listener to the close button
        closeButton.onClick.AddListener(HidePopup);
    }

    void Update()
    {
        if (_isRotating)
        {
            RotateCylinders();
            if (checkedForPatterns)
            {
                checkedForPatterns = false;
            }
        }
    }

    private void OnBetChanged()
    {
        _difficultyFactor = GameController.Instance._current_bet_index;
        RefreshCylinder();
    }

    public void Spin()
    {
        _spinButton.interactable = false;
        print("Turn OFF SPIN");

        GameController.Instance._increase_bet_button.interactable = false;
        GameController.Instance._decrease_bet_button.interactable = false;
        if (!_betConfirmed) return;
        if (_isRotating && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            StopNextCylinder();
        }
        else
        {
            if (!_isRotating)
            {
                StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
                OnStartSpinning();
            }

            if (fxPrefab != null)
            {
                // Instantiate the FX Prefab at the button's position
                GameObject fxInstance = Instantiate(fxPrefab, spinButtonDup.transform.position, Quaternion.identity);
                slotAudio.Play();
                // Optionally, you can destroy the fxInstance after a certain duration
                Destroy(fxInstance, 2f); // Adjust the duration as needed
            }
        }
    }

    void RotateCylinders()
    {
        for (int i = 0; i < numberOfCylinders; i++)
        {
            if (cylinderRotationStates[i])
            {
                cylinderParents[i].Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

                // Check for collisions with centerpoint
                Collider[] colliders = cylinderParents[i].GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("centerpoint"))
                    {
                        // Check for collisions with the centerpoint
                        Collider[] symbolColliders =
                            Physics.OverlapBox(collider.transform.position, collider.bounds.extents);
                        foreach (Collider symbolCollider in symbolColliders)
                        {
                            // Adjust the rotation to align the symbol with the center
                            if (symbolCollider.CompareTag("SymbolImage"))
                            {
                                AlignSymbolWithCenter(symbolCollider.transform, collider.transform);
                                StopNextCylinder(); // Stop the rotation for the current cylinder
                            }
                        }
                    }
                }
            }
        }
    }

    void AlignSymbolWithCenter(Transform symbol, Transform center)
    {
        // Calculate the rotation needed to align the symbol with the center
        Vector3 targetDirection = center.position - symbol.position;
        targetDirection.y = 0f; // Ignore the y-axis for rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Smoothly rotate the symbol towards the center
        StartCoroutine(RotateSymbolTowardsCenter(symbol, targetRotation, 1.0f));
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

    public void StartRotating()
    {
        _spinButton.interactable = false;
        print("Turn OFF SPIN in Rotating");
        //  check if balance is sufficient, done to remove dependence on gamecontroller -> isbalancesufficient
        float t_balance_after_test = PlayerPrefs.GetFloat(GameController.Instance._playerprefs_balance_key) -
                                     GameController.Instance._bet_intervals_in_usd[
                                         GameController.Instance._current_bet_index];

        //if (!GameController.Instance.Is_Balance_Sufficient)
        if (t_balance_after_test < 0)
        {
            UIManager.Instance.TriggerBalanceInsufficient();
            return;
        }
        else
        {
            //  apply deduction if balance ok
            PlayerPrefs.SetFloat(GameController.Instance._playerprefs_balance_key, t_balance_after_test);

            _difficultyFactor = GameController.Instance._current_bet_index;
            _betConfirmed = true;
        }

        if (!_isRotating)
        {
            //GameController.Instance.FinaliseBetOnClickSpin();
            print(t_balance_after_test);
            GameController.Instance._currentPoints = t_balance_after_test;
            PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);

            GameController.Instance._display_points = (int)Mathf.Round(GameController.Instance._currentPoints *
                                                                       GameController.Instance._point_multiplier);
            print(GameController.Instance._currentPoints);

            //GameController.Instance._currentPointsText.text = t_balance_after_test.ToString();
            print("Update Points ICS");
            print(GameController.Instance._display_points);
            GameController.Instance._currentPointsText.text = GameController.Instance._display_points.ToString();
            PlayerStats.Instance.HandsPlayed++;
            UIManager.Instance.HandsPlayedIncrement(PlayerStats.Instance.HandsPlayed.ToString());
        }
    }

    public void DisableCylinders()
    {
        cylinderParents[0].gameObject.SetActive(false);
        cylinderParents[4].gameObject.SetActive(false);
        hideImage.SetActive(false);
        hideImage1.SetActive(false);
        hideImage2.SetActive(false);
        hideImage3.SetActive(false);
        hideImage4.SetActive(false);
        hideImage5.SetActive(false);
    }

    public void EnableRevealImages()
    {
        hideImage.SetActive(true);
        hideImage1.SetActive(true);
        hideImage2.SetActive(true);
        hideImage3.SetActive(true);
        hideImage4.SetActive(true);
        hideImage5.SetActive(true);
    }

    internal void RefreshCylinder()
    {
        if (cylinderParents.Length > 0)
        {
            foreach (var parent in cylinderParents)
            {
                if (parent.childCount > 0)
                {
                    foreach (Transform child in parent)
                    {
                        Destroy(child.gameObject);
                    }
                }

                Destroy(parent.gameObject);
            }
        }

        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);
        for (int i = 0; i < numberOfCylinders; i++)
        {
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform);
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }
    }

    void SpawnCylinders()
    {
        //refreshBtn.gameObject.SetActive(true);
        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);

        for (int i = 0; i < numberOfCylinders; i++)
        {
            _cylinder_positions.Add(spawnPosition);
            print(spawnPosition);
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform); // Pass the parent transform
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }

        // Rotate the entire cylinder to make it vertical
        cylinderParents[numberOfCylinders - 1].rotation = Quaternion.Euler(0f, 0f, 0f);

        // Set the new parent for all cylinders
        allCylindersParent.transform.SetParent(transform);
        foreach (var parent in cylinderParents)
        {
            parent.SetParent(allCylindersParent.transform);
        }

        allCylindersParent.transform.position = new Vector3(3.5f, 0, 4.5f);
        allCylindersParent.transform.rotation = Quaternion.Euler(0, 0, +90);

        //_spinButton.gameObject.SetActive(true);
    }

    void SpawnImagesOnCylinder(Vector3 spawnPosition, int index, Transform parentTransform)
    {
        float angleIncrement = 360f / numberOfImages;

        GameObject cylinderParent = new GameObject("CylinderParent" + index);
        //cylinderParent.transform.position = cylinderSpawnPoints[index];
        cylinderParents[index] = cylinderParent.transform; // Store the reference to the parent
        cylinderRotationStates[index] = true; // Set the initial rotation state to true

        // Set the parent transform for the cylinder
        cylinderParent.transform.SetParent(parentTransform);
        for (int i = 0; i < numberOfImages; i++)
        {
            float angle = i * angleIncrement;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * cylinderRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * cylinderRadius;

            Vector3 imageSpawnPosition =
                spawnPosition + new Vector3(x, 0f, z); // Offset image position relative to cylinder
            Quaternion spawnRotation = Quaternion.Euler(0f, -angle, 0f);

            //    ServiceLocator.Instance.Get<RngGenerator>().RandInt(0, imagePrefabs.Length - 5 + GameController.Instance.CurrentBetIndex, (int)(i * (spawnPosition.x + spawnPosition.y + spawnPosition.z) / 2));

            GameObject imageObject = Instantiate(imagePrefabs[Random.Range(0, imagePrefabs.Length)], imageSpawnPosition,
                spawnRotation);
            // GameObject imageObject = Instantiate(GetWeightedRNG.GetValue(slotRNGItems.ItemsForRNG), imageSpawnPosition, spawnRotation);
            imageObject.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            _spawnedIcons[i] = imageObject.GetComponent<Icons>();
            imageObject.transform.parent = cylinderParent.transform; // Set the cylinder's parent as the parent

            // Calculate the angle between the center and the current image
            float angleToCenter =
                Mathf.Atan2(imageSpawnPosition.z - spawnPosition.z, imageSpawnPosition.x - spawnPosition.x) *
                Mathf.Rad2Deg;
            // Rotate the image to face the center
            imageObject.transform.rotation = Quaternion.Euler(180, 90 - angleToCenter, 90);
        }

        Debug.Log("Spawn " + gameObject);
        cylinderParent.transform.localPosition = Vector3.zero; // Set local position to zero
        cylinderParent.transform.localRotation = Quaternion.identity; // Set local rotation to identity
        cylinderParents[index].localScale = new Vector3(1, 1, 1);
    }

    void StartRotatingCylinders()
    {
        _isRotating = true;
        _currentCylinder = 0;
    }


    public void StopCylinderAtIndex(int stopCylinderAtIndex)
    {
        if (stopCylinderAtIndex < numberOfCylinders && cylinderRotationStates[stopCylinderAtIndex])
        {
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            float TargetRot = cylinderParents[stopCylinderAtIndex].localRotation.eulerAngles.y;
            foreach (var element in rotations)
            {
                var difference = Math.Abs((double)element - TargetRot);
                if (minDifference > difference)
                {
                    minDifference = (float)difference;
                    closest = element;
                }
            }

            cylinderRotationStates[stopCylinderAtIndex] = false; // Stop the rotation for the current cylinder
            cylinderParents[stopCylinderAtIndex].localRotation = Quaternion.Euler(0, closest, 0);

            bool isEnd = true;
            for (int i = 0; i < numberOfCylinders; i++)
            {
                if (cylinderRotationStates[i] == true)
                {
                    isEnd = false;
                }
            }

            if (isEnd)
            {
                OnEndSpinning();
            }
        }
    }

    void StopNextCylinder()
    {
        int selectCylinder = _currentCylinder;
        while (selectCylinder < numberOfCylinders)
        {
            if (cylinderRotationStates[selectCylinder] == false)
            {
                selectCylinder++;
            }
            else
            {
                break;
            }
        }


        if (_currentCylinder < numberOfCylinders)
        {
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            float TargetRot = cylinderParents[_currentCylinder].localRotation.eulerAngles.y;
            foreach (var element in rotations)
            {
                var difference = Math.Abs((double)element - TargetRot);
                if (minDifference > difference)
                {
                    minDifference = (float)difference;
                    closest = element;
                }
            }

            cylinderRotationStates[_currentCylinder] = false; // Stop the rotation for the current cylinder
            cylinderParents[_currentCylinder].localRotation = Quaternion.Euler(0, closest, 0);

            if (currentSpinJackpot != JackpotTypes.None)
            {
                cylinderParents[_currentCylinder].localRotation = Quaternion.identity;
            }

            _currentCylinder++;

            if (_currentCylinder >= numberOfCylinders)
            {
                OnEndSpinning();
            }
        }
    }

    #region Cylinder Spinning

    private void OnStartSpinning()
    {
        currentSpinJackpot = GetWeightedRNG.GetValue(jackpotRNGItems.ItemsForRNG);

        print(currentSpinJackpot);

        if (currentSpinJackpot != JackpotTypes.None)
        {
            List<SpriteRenderer> slots = new List<SpriteRenderer>();
            
            FillOrigins();

            foreach (Transform raycastPoint in jackpotOccurPosition)
            {
                RaycastHit2D rayHit = Physics2D.Raycast(raycastPoint.position, Vector3.forward, _imageLayer);

                Debug.DrawLine(raycastPoint.position, rayHit.point, Color.blue);


                if (rayHit)
                {
                    SpriteRenderer slotRenderer = rayHit.collider.GetComponent<SpriteRenderer>();
                    if (slotRenderer)
                        slots.Add(slotRenderer);
                }
            }

            switch (currentSpinJackpot) // Align sprites in a line to match
        {
            case JackpotTypes.Minor:
                // Randomly select one function to call for Minor
                int minorSelection = Random.Range(0, 4); // Generates a random number between 0 and 3
                switch (minorSelection)
                {
                    case 0:
                        ModifySprites(slotRNGItems.ItemsForRNG[8].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 1:
                        ModifySprites(slotRNGItems.ItemsForRNG[9].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 2:
                        ModifySprites(slotRNGItems.ItemsForRNG[10].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 3:
                        ModifySprites(slotRNGItems.ItemsForRNG[11].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                }
                break;

            case JackpotTypes.Major:
                // Randomly select one function to call for Major
                int majorSelection = Random.Range(0, 4); // Generates a random number between 0 and 3
                switch (majorSelection)
                {
                    case 0:
                        ModifySprites(slotRNGItems.ItemsForRNG[4].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 1:
                        ModifySprites(slotRNGItems.ItemsForRNG[5].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 2:
                        ModifySprites(slotRNGItems.ItemsForRNG[6].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 3:
                        ModifySprites(slotRNGItems.ItemsForRNG[7].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                }
                break;

            case JackpotTypes.Grand:
                // Randomly select one function to call for Grand
                int grandSelection = Random.Range(0, 4); // Generates a random number between 0 and 3
                switch (grandSelection)
                {
                    case 0:
                        ModifySprites(slotRNGItems.ItemsForRNG[0].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 1:
                        ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 2:
                        ModifySprites(slotRNGItems.ItemsForRNG[2].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 3:
                        ModifySprites(slotRNGItems.ItemsForRNG[3].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                }
                break;

            case JackpotTypes.FreeSpin:
                // Randomly select one function to call for FreeSpin
                int freeSpinSelection = Random.Range(0, 4); // Generates a random number between 0 and 3
                switch (freeSpinSelection)
                {
                    case 0:
                        ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 1:
                        ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 2:
                        ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                    case 3:
                        ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);
                        break;
                }
                break;
        }
        }

        StartRotatingCylinders();
        StartCoroutine(StopCylindersSequentially()); // Start stopping the cylinders sequentially
        //CheckForWinningPatterns.INSTANCE.CheckPatterns();
    }

    private void FillOrigins()
    {
        //todo get positions random here
        jackpotOccurPosition.Clear();
        var randomInt = Random.Range(0, checkForWinningPatterns.raycastPatterns.Count);
        var element = checkForWinningPatterns.raycastPatterns[randomInt];
        foreach (var origins in element.raycastOrigins)
        {
            jackpotOccurPosition.Add(origins);
        }
    }

    private void OnEndSpinning()
    {
        if (_isRotating)
        {
            if (!CheckForWinningPatterns.INSTANCE.isBonus)
                StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
        }

        //if (currentSpinJackpot != JackpotTypes.None)
        //{
        //    List<SpriteRenderer> slots = new List<SpriteRenderer>();

        //    foreach (Transform raycastPoint in jackpotOccurPosition)
        //    {
        //        RaycastHit2D rayHit = Physics2D.Raycast(raycastPoint.position, Vector3.forward, _imageLayer);

        //        Debug.DrawLine(raycastPoint.position, rayHit.point, Color.blue);


        //        if (rayHit)
        //        {
        //            SpriteRenderer slotRenderer = rayHit.collider.GetComponent<SpriteRenderer>();
        //            if (slotRenderer)
        //                slots.Add(slotRenderer);
        //        }
        //    }

        //    switch (currentSpinJackpot)
        //    {
        //        case JackpotTypes.Minor:

        //            ModifySprites(slotRNGItems.ItemsForRNG[2].Item.GetComponent<SpriteRenderer>().sprite, slots);

        //            break;

        //        case JackpotTypes.Major:

        //            ModifySprites(slotRNGItems.ItemsForRNG[1].Item.GetComponent<SpriteRenderer>().sprite, slots);

        //            break;

        //        case JackpotTypes.Grand:

        //            ModifySprites(slotRNGItems.ItemsForRNG[0].Item.GetComponent<SpriteRenderer>().sprite, slots);

        //            break;
        //    }
        //}

        _isRotating = false;
        _currentCylinder = 0;
        slotAudio.Stop();
        for (int i = 0; i < numberOfCylinders; i++)
        {
            cylinderRotationStates[i] = true;
            //AlignSymbolWithClosestCenter(i);
        }

        _spinButton.gameObject.SetActive(false);

        //TODO:Winning Checked Here
        CheckWinningCondition();
        _betConfirmed = false;
    }

    private void ModifySprites(Sprite sprite, List<SpriteRenderer> renderingPositions)
    {
        foreach (SpriteRenderer renderer in renderingPositions)
        {
            renderer.sprite = sprite;
        }
    }

    #endregion

    #region Pattern detection effect

    public void OnPatternNotFound()
    {
        StartCoroutine(OnPatternNotFound(false));
    }

    private IEnumerator OnPatternNotFound(bool diffrentiate)
    {
        yield return StartCoroutine(DoorAnim.INSTANCE.DoorTrigger());
        GameController.Instance.RestartLevel.gameObject.SetActive(true);
    }

    #endregion

    #region Shift Slot Cylinder

    public void Shift()
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(true);
        }
    }

    public async void ShiftUp(int cylinderCount)
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);
            cylinderParents[cylinderCount].localRotation = Quaternion.AngleAxis(360 / numberOfImages, Vector3.down) *
                                                           cylinderParents[cylinderCount].localRotation;
            //for (float i = 0; i <= 1; i += 0.01f)
            //{
            //    if (!Application.isPlaying)
            //        break;


            //    await Task.Delay((int)_swapSpeed * 100);
            //}
            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }

    public void Shift_Left()
    {
        _button_left.interactable = false;
        _button_right.interactable = false;
        StartCoroutine(Cor_Shift_Left());
    }

    public void Shift_Right()
    {
        _button_left.interactable = false;
        _button_right.interactable = false;
        StartCoroutine(Cor_Shift_Right());
    }

    IEnumerator Cor_Shift_Left()
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);
            Vector3[] startPositions = new Vector3[4];
            Vector3[] targetPositions = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                startPositions[i] = cylinderParents[i].position;
                int nextIndex = (i + 1) % 4;
                targetPositions[i] = new Vector3(
                    i == 0 ? _cylinder_positions[0].y - 1.7f : 3.3f,
                    cylinderParents[nextIndex].position.y,
                    cylinderParents[nextIndex].position.z
                );
            }

            float shiftDuration = _shift_duration; // Replace with your actual shift duration
            float elapsedTime = 0f;

            while (elapsedTime < shiftDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / shiftDuration);

                for (int i = 0; i < 4; i++)
                {
                    cylinderParents[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                }

                yield return null;
            }

            // Ensure final positions are exact
            for (int i = 0; i < 4; i++)
            {
                cylinderParents[i].position = targetPositions[i];
            }

            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }

    IEnumerator Cor_Shift_Right()
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);
            Vector3[] startPositions = new Vector3[4];
            Vector3[] targetPositions = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                startPositions[i] = cylinderParents[i].position;
                int prevIndex = (i - 1 + 4) % 4;
                targetPositions[i] = new Vector3(
                    i == 3 ? _cylinder_positions[3].y + 0.4f : 1.4f,
                    cylinderParents[prevIndex].position.y,
                    cylinderParents[prevIndex].position.z
                );
            }

            float shiftDuration = _shift_duration; // Replace with your actual shift duration
            float elapsedTime = 0f;

            while (elapsedTime < shiftDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / shiftDuration);

                for (int i = 0; i < 4; i++)
                {
                    cylinderParents[i].position = Vector3.Lerp(startPositions[i], targetPositions[i], t);
                }

                yield return null;
            }

            // Ensure final positions are exact
            for (int i = 0; i < 4; i++)
            {
                cylinderParents[i].position = targetPositions[i];
            }

            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }

    /*
    IEnumerator Cor_Shift_Left()
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);

            List<Vector3> t_target_positions = new List<Vector3>
            {
                new Vector3(_cylinder_positions[0].y - 1.7f, cylinderParents[0].position.y, cylinderParents[0].position.z),
                new Vector3(3.3f, cylinderParents[1].position.y, cylinderParents[1].position.z),
                new Vector3(3.3f, cylinderParents[2].position.y, cylinderParents[2].position.z),
                new Vector3(3.3f, cylinderParents[3].position.y, cylinderParents[3].position.z)
            };

            float t_timer = _shift_duration;

            while (t_timer > 0)
            {
                t_timer -= Time.deltaTime;
                for (int i = 0; i < t_target_positions.Count; ++i)
                {
                    int j = i + 1;
                    if (j == t_target_positions.Count)
                    {
                        j = 0;
                    }
                    cylinderParents[i].position = Vector3.Lerp(cylinderParents[i].position, t_target_positions[i], Time.deltaTime);
                }
                yield return null;
            }

            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }
    
    IEnumerator Cor_Shift_Right()
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);

            List<Vector3> t_target_positions = new()
            {
                new Vector3(1.4f, cylinderParents[0].position.y, cylinderParents[0].position.z),
                new Vector3(1.4f, cylinderParents[1].position.y, cylinderParents[1].position.z),
                new Vector3(1.4f, cylinderParents[2].position.y, cylinderParents[2].position.z),
                new Vector3(_cylinder_positions[3].y + 0.4f, cylinderParents[3].position.y, cylinderParents[3].position.z),
            };

            float t_timer = _shift_duration;

            while (t_timer > 0)
            {
                t_timer -= Time.deltaTime;
                for (int i = 0; i < t_target_positions.Count; ++i)
                {
                    int j = i + 1;
                    if (j == t_target_positions.Count)
                    {
                        j = 0;
                    }
                    cylinderParents[i].position = Vector3.Lerp(cylinderParents[i].position, t_target_positions[i], Time.fixedDeltaTime);
                }
                yield return null;
            }

            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }
    */
    public async void ShiftDown(int cylinderCount)
    {
        if (CanShiftCylinder && !DoorAnim.INSTANCE.IsAnimRunning)
        {
            _shiftingPanel.SetActive(false);
            cylinderParents[cylinderCount].localRotation = Quaternion.AngleAxis(360 / numberOfImages, Vector3.up) *
                                                           cylinderParents[cylinderCount].localRotation;
            //for (float i = 0; i <= 1; i += 0.01f)
            //{
            //    if (!Application.isPlaying)
            //        break;


            //    await Task.Delay((int)_swapSpeed * 100);
            //}
            CanShiftCylinder = false;
            IsLastChanceToWin = true;
            CheckWinningCondition();
        }
    }

    #endregion

    void CylinderStopSpeed(float newSpeed)
    {
        CylinderStopInterval = newSpeed;
        // Update the speed text
        speedText.text = $"Delay: {CylinderStopInterval}";
    }

    IEnumerator StopCylindersSequentially()
    {
        for (int i = 0; i < numberOfCylinders; i++)
        {
            if (won)
            {
                won = false;
                yield break;
            }
            // Stop the rotation for the current cylinder
            //cylinderRotationStates[i] = false;

            // Delay for a short duration before stopping the next cylinder
            yield return new WaitForSeconds(CylinderStopInterval); // Adjust the duration as needed

            // Check for collisions with centerpoint
            if (_currentCylinder < numberOfCylinders)
            {
                var closest = float.MaxValue;
                var minDifference = float.MaxValue;
                float TargetRot = cylinderParents[_currentCylinder].localRotation.eulerAngles.y;
                foreach (var element in rotations)
                {
                    var difference = Math.Abs((double)element - TargetRot);
                    if (minDifference > difference)
                    {
                        minDifference = (float)difference;
                        closest = element;
                    }
                }

                cylinderRotationStates[_currentCylinder] = false; // Stop the rotation for the current cylinder
                cylinderParents[_currentCylinder].localRotation = Quaternion.Euler(0, closest, 0);

                if (currentSpinJackpot != JackpotTypes.None)
                {
                    cylinderParents[_currentCylinder].localRotation = Quaternion.identity;
                }

                _currentCylinder++;

                if (_currentCylinder == numberOfCylinders)
                {
                    OnEndSpinning();
                }
            }
        }
    }

    void ChangeRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
        // Update the speed text
        speedText.text = $"Speed: {rotationSpeed}";
    }


    void UpdateCoinsText()
    {
        // Update the coins text
        //coinsText.text = $"Coins: {coins}";
    }

    void ToggleMute()
    {
        isMuted = !isMuted; // Toggle the mute state

        // Adjust the volume based on the mute state
        AudioListener.volume = isMuted ? 0.1f : 1.0f;

        // Update the button text
        muteButton.GetComponentInChildren<Text>().text = isMuted ? "Unmute" : "Mute";
    }

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

    public void RefreshImages()
    {
        if (!_isRotating)
        {
            // Destroy existing image symbols
            if (!CylinderSpawning)
            {
                refreshBtn.interactable = false;
                CylinderSpawning = true;
                StartCoroutine(nameof(RefreshImagesFX));
            }
        }
    }

    public void RefreshImagesAgain()
    {
        if (!_isRotating)
        {
            // Destroy existing image symbols
            if (!CylinderSpawning)
            {
                CylinderSpawning = true;

                foreach (var parent in cylinderParents)
                {
                    foreach (Transform child in parent)
                    {
                        if (!child.gameObject.activeInHierarchy)
                        {
                            child.gameObject.SetActive(true);
                        }

                        child.gameObject.AddComponent<Rigidbody2D>();
                        Destroy(child.gameObject, 2f);
                    }

                    Destroy(parent.gameObject, 2f);
                }

                transform.localPosition = _cylinderInitialPos;
                SpawnCylinders();

                refreshBtn.interactable = true;
            }
        }
    }

    private IEnumerator RefreshImagesFX()
    {
        //destroy previous images fx
        foreach (var parent in cylinderParents)
        {
            foreach (Transform child in parent)
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(true);
                }

                child.gameObject.AddComponent<Rigidbody2D>();
                Destroy(child.gameObject, 2f);
            }

            Destroy(parent.gameObject, 2f);
        }

        yield return new WaitForSeconds(2);
        //create new images fx
        // Spawn new image symbols inside allCylindersParent
        Vector3 spawnPosition = new Vector3(0, distanceBetweenCylinders, 0);

        currentSpinJackpot = GetWeightedRNG.GetValue(jackpotRNGItems.ItemsForRNG);
        for (int i = 0; i < numberOfCylinders; i++)
        {
            SpawnImagesOnCylinder(spawnPosition, i, allCylindersParent.transform);
            spawnPosition += new Vector3(0f, distanceBetweenCylinders, 0f);
        }

        CylinderSpawning = false;
    }

    public void CheckWinningCondition()
    {
        if (CheckForWinningPatterns.INSTANCE != null)
        {
            CheckForWinningPatterns.INSTANCE.CheckPatterns();
            checkedForPatterns = true;
        }
    }

    public void StartBonusSpin()
    {
        StartCoroutine(FifteenBonusSpins());
    }


    public IEnumerator FifteenBonusSpins()
    {
        refreshBtn.interactable = false;
        bonusSpins = true;

        yield return StartCoroutine(RefreshImagesFX());

        StartRotatingCylinders();

        yield return StartCoroutine(StopCylindersSequentially());


        yield return null;
        bonusSpins = false;
    }
}