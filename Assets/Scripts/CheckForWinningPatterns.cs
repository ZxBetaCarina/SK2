using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class RaycastOriginTransforms
{
    public string listName;
    public List<Transform> raycastOrigins;
}

public enum JackpotTypes
{
    None,
    Minor,
    Major,
    Grand,
    FreeSpin
}

public class CheckForWinningPatterns : MonoBehaviour
{
    public List<Transform> ForthRow;

    public List<RaycastOriginTransforms> raycastPatterns;
    public static CheckForWinningPatterns INSTANCE;
    [SerializeField] private GameObject _lastRowParent;
    [SerializeField] private GameObject leftSpins;
    [SerializeField] private TMP_Text leftSpinsText;
    [SerializeField] private GameObject reviewImgParent;
    [SerializeField] private Button reviewBtn;
    [SerializeField] private TMP_Text reviewText;

    [Header("Bonus and Jackpot Icon")]
    [SerializeField] private Sprite bonus1;
    [SerializeField] private Sprite bonus2;
    /*[SerializeField] private Sprite bonus2;
    [SerializeField] private Sprite bonus3;
    [Space, SerializeField] private Sprite jackpotMinor;
    [SerializeField] private Sprite jackpotMajor;
    [SerializeField] private Sprite jackpotGrand;*/

    public static Action CoolDownRubicButton;

    public static Action PatternFound;
    public static Action PatternNotFound;
    private int noOfPatterns = 0;
    private bool _isJackPotMode = false;
    private bool _checking = false;
    private bool isLastSpin = false;

    private bool isReviewActive = false;

    private int spinCounts = 15;

    public bool isBonus = false;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FinaliseBet()
    {
        reviewImgParent.SetActive(false);
        reviewBtn.gameObject.SetActive(false);
    }

    public void ReviewImages(bool ShowReview)
    {
        if (isReviewActive)
        {
            isReviewActive = false;
            return;
        }
        else if (!ShowReview)
        {
            return;
        }
        StartCoroutine(ShowImages());
    }


    IEnumerator ShowImages()
    {
        //reviewBtn.interactable = false;

        isReviewActive = true;

        reviewText.text = "Hide Preview";

        reviewImgParent.SetActive(false);

        float time = 5f;

        while (time > 0)
        {
            time -= .1f;
            if (!isReviewActive)
            {
                reviewImgParent.SetActive(true);

                reviewText.text = "Preview";

                yield break;
            }
            yield return new WaitForSeconds(.1f);
        }


        if (!ImageCylinderSpawner.INSTANCE._isRotating)
            reviewImgParent.SetActive(true);


        //reviewBtn.interactable = true;

        reviewText.text = "Review";

        isReviewActive = false;
    }

    private void OnEnable()
    {
        RubikCubeController.RubicMoved += OnRubicMoved;

        leftSpins.SetActive(false);
    }

    private void OnDisable()
    {
        RubikCubeController.RubicMoved -= OnRubicMoved;
    }

    private void OnRubicMoved()
    {
        if (!_checking)
            StartCoroutine(nameof(CheckForPatterns));
    }

    public IEnumerator CheckForPatterns()
    {
        if (_checking) yield break;


        _checking = true;
        //yield return new WaitForSeconds(.1f);

        foreach (var raycastOrigins in raycastPatterns)
        {
            CheckPatternsInList(raycastOrigins.raycastOrigins);
            //yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Total Patterns = " + noOfPatterns);

        if (isBonus)
        {

            if (noOfPatterns >= 1)
            {
                PatternFound?.Invoke();
                ImageCylinderSpawner.INSTANCE.won = false;
            }
            yield return new WaitForSeconds(2f);
            print("Start Fifteen spins");
            ImageCylinderSpawner.INSTANCE.StartBonusSpin();

            spinCounts--;

            leftSpinsText.text = spinCounts + "/" + 15;

            _checking = false;



            if (spinCounts < 1)
            {
                isBonus = false;
                isLastSpin = true;
            }


            noOfPatterns = 0;
            yield break;
        }

        /*if (isBonus)
        {
            isBonus = false;
        }*/

        if (noOfPatterns >= 1 && !_isJackPotMode)
        {
            PatternFound?.Invoke();
        }
        else if (noOfPatterns >= 1 && _isJackPotMode)
        {
            PatternFound?.Invoke();
            GameController.Instance.JackPotWinning();
        }
        else
        {
            if (isLastSpin)
            {
                leftSpins.SetActive(false);
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene(1);
            }
            else
            {
                PatternNotFound?.Invoke();

            }
        }
        _isJackPotMode = false;

        noOfPatterns = 0;
        _checking = false;
        CoolDownRubicButton?.Invoke();
    }

    public void DisableLastRow()
    {
        _lastRowParent.SetActive(false);
    }

    public void CheckPatterns()
    {
        if (!_checking)
        {
            _isJackPotMode = GameController.Instance.JackPotMode;
            StartCoroutine(CheckForPatterns());
        }
    }

    private void CheckPatternsInList(List<Transform> transformList)
    {
        int matchCount = 0;
        Sprite prevSprite = null;
        int listsize = transformList.Count;
        List<Vector3> detected = new List<Vector3>();

        //Find patterns in ascending list order
        for (int i = 0; i < transformList.Count; i++)
        {
            if (!transformList[i].gameObject.activeInHierarchy)
            {
                //Debug.Log("Continued");
                continue;
            }
            RaycastHit2D detectedObject = Physics2D.Raycast(transformList[i].position, Vector3.forward);
            //detect first image for the pattern
            if (matchCount == 0)
            {
                if (detectedObject.transform != null)
                {
                    //matchCount = 1 for first detected image in the pattern
                    matchCount++;
                    prevSprite = detectedObject.collider.GetComponent<SpriteRenderer>().sprite;
                    detected.Add(detectedObject.transform.position);
                }
            }

            //already detected first image 
            else
            {

                if (detectedObject.transform != null) // 3*3
                {
                    //check name of previous image and newly detected image
                    if (prevSprite == detectedObject.collider.GetComponent<SpriteRenderer>().sprite)
                    {
                        matchCount++;
                        detected.Add(detectedObject.transform.position);
                    }
                    else
                    {
                        //name of images are not matching
                        //strart count from one and check for other pattern
                        matchCount = 1;
                        prevSprite = detectedObject.collider.GetComponent<SpriteRenderer>().sprite;
                        detected = new List<Vector3>();
                        detected.Add(detectedObject.transform.position);
                    }
                    if (matchCount >= 3 && i == transformList.Count - 1)
                    {
                        noOfPatterns++;
                        Debug.Log(detectedObject.transform.localPosition + " matched " + prevSprite);

                        if (detectedObject.collider.GetComponent<SpriteRenderer>().sprite == bonus1)
                        {
                            StartCoroutine(DelayFreeSpin());                                        // Enable FREE Spin
                        }
                        else if (detectedObject.collider.GetComponent<SpriteRenderer>().sprite == bonus2)
                        {
                            StartCoroutine(Bonus2start());                                                               // Enable Rainbow Jump
                        }
                        else
                        {

                            foreach (var pattern in detected)
                            {
                                Debug.Log(detected.Count);
                                if (GameController.Instance != null)
                                {
                                    GameController.Instance._patterns.Add(pattern);
                                    Debug.Log("Pattern Found");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    
    private IEnumerator Bonus2start()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);
    }

    private IEnumerator DelayFreeSpin()
    {
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowFreeSpin();
        StartCoroutine(ShowDelaysSpinsLeft());
        print("Is Bonus True");
        isBonus = true;
        StartCoroutine(CheckForPatterns());
    }

    IEnumerator ShowDelaysSpinsLeft()
    {
        yield return new WaitForSeconds(2f);
        leftSpins.SetActive(true);
        leftSpinsText.text = 15 + "/" + 15;
    }

    private IEnumerator BonusThird()
    {
        //Show Bonus UI
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Bonus 3");
    }

    public void OnRevealEnabled()
    {
        while (_checking)
        {

        }

        StartCoroutine(nameof(CheckForPatterns));
    }
}
