using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SK2
{
    [Serializable]
    public class RaycastOriginTransforms
    {
        public string listName;
        public List<Transform> raycastOrigins;
    }
    public class PatternDetector : MonoBehaviour
    {
        [SerializeField] private GameObject raycastPoint;

        public List<RaycastOriginTransforms> raycastPatterns;
        public static PatternDetector INSTANCE;
        [SerializeField] private GameObject _lastRowParent;
        [SerializeField] private GameObject leftSpins;
        [SerializeField] private TMP_Text leftSpinsText;
        [SerializeField] private GameObject reviewImgParent;
        [SerializeField] private Button reviewBtn;
        [SerializeField] private TMP_Text reviewText;

        public static Action CoolDownRubicButton;
        private ImageCylinderSpawner ics;

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
            print("Init");
            if (INSTANCE == null)
            {
                INSTANCE = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            ics = GetComponent<ImageCylinderSpawner>();    
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

            //reviewBtn.interactable = true;

            reviewText.text = "Review";

            isReviewActive = false;
        }

        private void OnEnable()
        {
            //RubikCubeController.RubicMoved += OnRubicMoved;

            leftSpins.SetActive(false);
        }

        private void OnDisable()
        {
            //RubikCubeController.RubicMoved -= OnRubicMoved;
        }

        //private void OnRubicMoved()
        //{
        //    if (!_checking)
        //        StartCoroutine(nameof(CheckForPatterns));
        //}

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

                ImageCylinderSpawner.INSTANCE.StartBonusSpin();

                spinCounts--;

                leftSpinsText.text = spinCounts + "/" + 15;

                _checking = false;



                if (spinCounts < 1)
                {
                    isBonus = false;
                    spinCounts = 15;
                    isLastSpin = true;
                }
                


                noOfPatterns = 0;
                yield break;
            }

            if (isBonus)
            {
                isBonus = false;
            }

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
                    Debug.Log("CurrentBetIndex 6 "+PlayerPrefs.GetFloat("CurrentBetIndex")); 
                    SceneManager.LoadScene("Level 1");
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
            string previousImageName = null;
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
                Vector3 position = new Vector3(transformList[i].position.x, transformList[i].position.y, -10);
                RaycastHit2D detectedObject = Physics2D.Raycast(transformList[i].position, Vector3.forward);
                //detect first image for the pattern
                if (matchCount == 0)
                {
                    if (detectedObject.transform != null)
                    {
                        //matchCount = 1 for first detected image in the pattern
                        matchCount++;
                        previousImageName = detectedObject.transform.gameObject.name;
                        detected.Add(detectedObject.transform.position);
                        Debug.Log("detected object 1  "+detected);
                    }
                }

                //already detected first image 
                else
                {

                            print("Ree spin 0");
                    if (detectedObject.transform != null) // 3*3
                    {
                        //check name of previous image and newly detected image
                        if (previousImageName == detectedObject.transform.gameObject.name)
                        {
                            matchCount++;
                            detected.Add(detectedObject.transform.position);
                           
                            
                        }
                        else
                        {
                            //name of images are not matching
                            //start count from one and check for other pattern
                            matchCount = 1;
                            previousImageName = detectedObject.transform.gameObject.name;
                            detected = new List<Vector3>();
                            
                        }
                        if (matchCount >= 3)
                        {
                            noOfPatterns++;
                            print("Ree spin 1");
                            if (detectedObject.transform.name == "_Type10(Clone)")
                            {
                                UIManager.Instance.ShowFreeSpin();
                                print("Ree spin 2");

                                StartCoroutine(ShowDelaysSpinsLeft());

                                isBonus = true;
                                //SceneManager.LoadScene
                            }
                            else if (detectedObject.transform.name == "Bonus 02(Clone)")
                            {
                                isBonus = true;
                                //StartCoroutine(BonusSecond());
                            }
                            else if (detectedObject.transform.name == "Bonus 03(Clone)")
                            {
                                isBonus = true;
                                //StartCoroutine(BonusThird());
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


        IEnumerator ShowDelaysSpinsLeft()
        {
            yield return new WaitForSeconds(2f);
            leftSpins.SetActive(true);
            leftSpinsText.text = 15 + "/" + 15;
        }

        private IEnumerator BonusSecond()
        {
            //Show Bonus UI
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("Bonus 2");
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

}