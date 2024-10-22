using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ImagesStrings
{
    public string name;
    public Sprite sprite;
}
public class RubikCubeController : MonoBehaviour
{
    [SerializeField] private List<Transform> originTransforms;


    #region SlotImages

    [SerializeField] private List<Sprite> spriteCollection = new();
    [SerializeField] private List<SpriteRenderer> upLayer1 = new();
    [SerializeField] private List<SpriteRenderer> upLayer2 = new();
    [SerializeField] private List<SpriteRenderer> upLayer3 = new();
    [SerializeField] private List<SpriteRenderer> upLayer4 = new();
    [SerializeField] private List<SpriteRenderer> downLayer1 = new();
    [SerializeField] private List<SpriteRenderer> downLayer2 = new();
    [SerializeField] private List<SpriteRenderer> downLayer3 = new();
    [SerializeField] private List<SpriteRenderer> downLayer4 = new();
    [SerializeField] private List<SpriteRenderer> leftLayer1 = new();
    [SerializeField] private List<SpriteRenderer> leftLayer2 = new();
    [SerializeField] private List<SpriteRenderer> leftLayer3 = new();
    [SerializeField] private List<SpriteRenderer> leftLayer4 = new();
    [SerializeField] private List<SpriteRenderer> rightLayer1 = new();
    [SerializeField] private List<SpriteRenderer> rightLayer2 = new();
    [SerializeField] private List<SpriteRenderer> rightLayer3 = new();
    [SerializeField] private List<SpriteRenderer> rightLayer4 = new();
    [SerializeField] private List<SpriteRenderer> centerVertical1 = new();
    [SerializeField] private List<SpriteRenderer> centerVertical2 = new();
    [SerializeField] private List<SpriteRenderer> centerVertical3 = new();
    [SerializeField] private List<SpriteRenderer> centerVertical4 = new();
    [SerializeField] private List<SpriteRenderer> centerHorizontal1 = new();
    [SerializeField] private List<SpriteRenderer> centerHorizontal2 = new();
    [SerializeField] private List<SpriteRenderer> centerHorizontal3 = new();
    [SerializeField] private List<SpriteRenderer> centerHorizontal4 = new();
    [SerializeField] private List<SpriteRenderer> frontSprites = new();
    #endregion



    #region CubeImages

    [SerializeField] private List<SpriteRenderer> CubeUpLayer1 = new();
    [SerializeField] private List<SpriteRenderer> CubeUpLayer2 = new();
    [SerializeField] private List<SpriteRenderer> CubeUpLayer3 = new();
    [SerializeField] private List<SpriteRenderer> CubeUpLayer4 = new();
    [SerializeField] private List<SpriteRenderer> CubeDownLayer1 = new();
    [SerializeField] private List<SpriteRenderer> CubeDownLayer2 = new();
    [SerializeField] private List<SpriteRenderer> CubeDownLayer3 = new();
    [SerializeField] private List<SpriteRenderer> CubeDownLayer4 = new();
    [SerializeField] private List<SpriteRenderer> CubeLeftLayer1 = new();
    [SerializeField] private List<SpriteRenderer> CubeLeftLayer2 = new();
    [SerializeField] private List<SpriteRenderer> CubeLeftLayer3 = new();
    [SerializeField] private List<SpriteRenderer> CubeLeftLayer4 = new();
    [SerializeField] private List<SpriteRenderer> CubeRightLayer1 = new();
    [SerializeField] private List<SpriteRenderer> CubeRightLayer2 = new();
    [SerializeField] private List<SpriteRenderer> CubeRightLayer3 = new();
    [SerializeField] private List<SpriteRenderer> CubeRightLayer4 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterVertical1 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterVertical2 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterVertical3 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterVertical4 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal1 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal2 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal3 = new();
    [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal4 = new();
    [SerializeField] private List<SpriteRenderer> CubeFrontFace = new();
    #endregion


    private List<SpriteRenderer> _detected = new();

    [SerializeField] private List<ImagesStrings> imagesStrings = new();
    [SerializeField] private List<GameObject> _selectedFace = new();
    [SerializeField] private SelectFace _cubeSelectFace;
    [SerializeField] private CubeState _cubeState;
    [SerializeField] private ReadCube _readCube;

    public List<SpriteRenderer> spriteRenderers;
    public List<SpriteRenderer> OldSprites;
    public List<Vector3> spritePositions;
    public List<GameObject> newSpriteObjects;
    public Transform originalParent;
    public Transform parentTransform;
    public float rotationSpeed = 30f; // Adjust this value as needed
    public float targetRotation = -90f; // Target rotation angle
    private bool shouldRotate = false;

    private int activeUpLayer = 0;
    private int activeDownLayer = 0;
    private int activeLeftLayer = 0;
    private int activeRightLayer = 0;
    private int activeCenterVerticalLayer = 0;
    private int activeCenterHorizontalLayer = 0;

    public static Action RubicMoved;
    public static Action StartCoolDown;

    private void OnEnable()
    {
        GameController.BetConfirmed += OnBetConfirmed;
    }

    private void OnDisable()
    {
        GameController.BetConfirmed -= OnBetConfirmed;
    }



    private void Start()
    {
        foreach (SpriteRenderer s in frontSprites)
        {
            s.gameObject.SetActive(false);
        }

    }



    private void UpdateCubeFrontFace()
    {
        for (int i = 0; i < CubeFrontFace.Count; i++)
        {
            if (_cubeState.front[i].transform.GetChild(0).GetComponent<SpriteRenderer>() != null) Debug.Log("found");
            frontSprites[i].sprite = _cubeState.front[i].GetComponentInChildren<Icons>().GetSprite();
            foreach (ImagesStrings iS in imagesStrings)
            {
                if (frontSprites[i].sprite.name == iS.sprite.name)
                {
                    frontSprites[i].gameObject.name = iS.name;
                }
            }
        }
    }


    private void OnBetConfirmed()
    {
        Invoke(nameof(UpdateCubeFrontFaceList), .2f);
    }

    private void UpdateCubeFrontFaceList()
    {
        for (int i = 0; i < _cubeState.front.Count; i++)
        {
            CubeFrontFace[i] = _cubeState.front[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
    }



    IEnumerator InvokeRubicMoved()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(.2f);
        //Note:
        //Wait because cube rotation is not done yet

        _readCube.ReadState();


        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        UpdateCubeFrontFace();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        RubicMoved?.Invoke();

    }



    public void IncreaseUpLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeUpLayer++;
        activeUpLayer %= 4;
        switch (activeUpLayer)
        {
            case 0:
                //for (int i = 0; i < upLayer1.Count; i++)
                //{
                //    upLayer4[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = upLayer4;
                spriteRenderers = upLayer1;
                break;
            case 1:
                //for (int i = 0; i < upLayer2.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(true);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = upLayer1;
                spriteRenderers = upLayer2;
                break;
            case 2:
                //for (int i = 0; i < upLayer2.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(true);
                //    upLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = upLayer2;
                spriteRenderers = upLayer3;
                break;
            case 3:
                //for (int i = 0; i < upLayer2.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = upLayer3;
                spriteRenderers = upLayer4;
                break;
        }

        _selectedFace = _cubeState.up;
        _cubeSelectFace.RotateSide(_cubeState.up, 1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void IncreaseDownLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeDownLayer++;
        activeDownLayer %= 4;
        switch (activeDownLayer)
        {
            case 0:
                //for (int i = 0; i < downLayer1.Count; i++)
                //{
                //    downLayer4[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = downLayer4;
                spriteRenderers = downLayer1;
                break;
            case 1:
                //for (int i = 0; i < downLayer2.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(true);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = downLayer1;
                spriteRenderers = downLayer2;
                break;
            case 2:
                //for (int i = 0; i < downLayer2.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(true);
                //    downLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = downLayer2;
                spriteRenderers = downLayer3;
                break;
            case 3:
                //for (int i = 0; i < downLayer2.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = downLayer3;
                spriteRenderers = downLayer4;
                break;

        }
        _selectedFace = _cubeState.down;
        _cubeSelectFace.RotateSide(_cubeState.down, 1);
        StartCoroutine(InvokeRubicMoved());
    }

    public void IncreaseLeftLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeLeftLayer++;
        activeLeftLayer %= 4;
        switch (activeLeftLayer)
        {
            case 0:
                //for (int i = 0; i < leftLayer1.Count; i++)
                //{
                //    leftLayer4[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = leftLayer4;
                spriteRenderers = leftLayer1;
                break;
            case 1:
                //for (int i = 0; i < leftLayer2.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(true);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = leftLayer1;
                spriteRenderers = leftLayer2;
                break;
            case 2:
                //for (int i = 0; i < leftLayer2.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(true);
                //    leftLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = leftLayer2;
                spriteRenderers = leftLayer3;
                break;
            case 3:
                //for (int i = 0; i < leftLayer2.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = leftLayer3;
                spriteRenderers = leftLayer4;
                break;

        }
        _selectedFace = _cubeState.left;
        _cubeSelectFace.RotateSide(_cubeState.left, 1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void IncreaseRightLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeRightLayer++;
        activeRightLayer %= 4;
        switch (activeRightLayer)
        {
            case 0:
                //for (int i = 0; i < rightLayer1.Count; i++)
                //{
                //    rightLayer4[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = rightLayer4;
                spriteRenderers = rightLayer1;
                break;
            case 1:
                //for (int i = 0; i < rightLayer2.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(true);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = rightLayer1;
                spriteRenderers = rightLayer2;
                break;
            case 2:
                //for (int i = 0; i < rightLayer3.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(true);
                //    rightLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = rightLayer2;
                spriteRenderers = rightLayer3;
                break;
            case 3:
                //for (int i = 0; i < rightLayer4.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = rightLayer3;
                spriteRenderers = rightLayer4;
                break;

        }
        _selectedFace = _cubeState.right;
        _cubeSelectFace.RotateSide(_cubeState.right, 1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void IncreaseCenterVertical(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeCenterVerticalLayer++;
        activeCenterVerticalLayer %= 4;
        switch (activeCenterVerticalLayer)
        {
            case 0:
                //for (int i = 0; i < centerVertical1.Count; i++)
                //{
                //    centerVertical4[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical1[i].gameObject.SetActive(true);
                //}
                OldSprites = centerVertical4;
                spriteRenderers = centerVertical1;
                break;
            case 1:
                //for (int i = 0; i < centerVertical2.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(true);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerVertical1;
                spriteRenderers = centerVertical2;
                break;
            case 2:
                //for (int i = 0; i < centerVertical3.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(true);
                //    centerVertical4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerVertical2;
                spriteRenderers = centerVertical3;
                break;
            case 3:
                //for (int i = 0; i < centerVertical4.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical4[i].gameObject.SetActive(true);
                //}
                OldSprites = centerVertical3;
                spriteRenderers = centerVertical4;
                break;

        }
        _selectedFace = _cubeState.centerVertical;
        _cubeSelectFace.RotateSide(_cubeState.centerVertical, -1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void IncreaseCenterHorizontal(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeCenterHorizontalLayer++;
        activeCenterHorizontalLayer %= 4;
        switch (activeCenterHorizontalLayer)
        {
            case 0:
                //for (int i = 0; i < centerHorizontal1.Count; i++)
                //{
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal1[i].gameObject.SetActive(true);
                //}
                OldSprites = centerHorizontal4;
                spriteRenderers = centerHorizontal1;
                break;
            case 1:
                //for (int i = 0; i < centerHorizontal2.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(true);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerHorizontal1;
                spriteRenderers = centerHorizontal2;
                break;
            case 2:
                //for (int i = 0; i < centerHorizontal3.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(true);
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerHorizontal2;
                spriteRenderers = centerHorizontal3;
                break;
            case 3:
                //for (int i = 0; i < centerHorizontal4.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal4[i].gameObject.SetActive(true);
                //}
                OldSprites = centerHorizontal3;
                spriteRenderers = centerHorizontal4;
                break;

        }
        _selectedFace = _cubeState.centerHorizontal;
        _cubeSelectFace.RotateSide(_cubeState.centerHorizontal, 1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void DecreaseUpLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeUpLayer--;
        if (activeUpLayer < 0)
        {
            activeUpLayer = 3;
        }
        switch (activeUpLayer)
        {
            case 0:
                //for (int i = 0; i < upLayer1.Count; i++)
                //{
                //    upLayer4[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = upLayer2;
                spriteRenderers = upLayer1;
                break;
            case 1:
                //for (int i = 0; i < upLayer2.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(true);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = upLayer3;
                spriteRenderers = upLayer2;
                break;
            case 2:
                //for (int i = 0; i < upLayer3.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(true);
                //    upLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = upLayer4;
                spriteRenderers = upLayer3;
                break;
            case 3:
                //for (int i = 0; i < upLayer4.Count; i++)
                //{
                //    upLayer1[i].gameObject.SetActive(false);
                //    upLayer2[i].gameObject.SetActive(false);
                //    upLayer3[i].gameObject.SetActive(false);
                //    upLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = upLayer1;
                spriteRenderers = upLayer4;
                break;

        }
        _selectedFace = _cubeState.up;
        _cubeSelectFace.RotateSide(_cubeState.up, -1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void DecreaseDownLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeDownLayer--;
        if (activeDownLayer < 0)
        {
            activeDownLayer = 3;
        }
        switch (activeDownLayer)
        {
            case 0:
                //for (int i = 0; i < downLayer1.Count; i++)
                //{
                //    downLayer4[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = downLayer2;
                spriteRenderers = downLayer1;
                break;
            case 1:
                //for (int i = 0; i < downLayer2.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(true);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = downLayer3;
                spriteRenderers = downLayer2;
                break;
            case 2:
                //for (int i = 0; i < downLayer3.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(true);
                //    downLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = downLayer4;
                spriteRenderers = downLayer3;
                break;
            case 3:
                //for (int i = 0; i < downLayer4.Count; i++)
                //{
                //    downLayer1[i].gameObject.SetActive(false);
                //    downLayer2[i].gameObject.SetActive(false);
                //    downLayer3[i].gameObject.SetActive(false);
                //    downLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = downLayer1;
                spriteRenderers = downLayer4;
                break;

        }
        _selectedFace = _cubeState.down;
        _cubeSelectFace.RotateSide(_cubeState.down, -1);
        StartCoroutine(InvokeRubicMoved());
    }

    public void DecreaseLeftLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeLeftLayer--;
        if (activeLeftLayer < 0)
        {
            activeLeftLayer = 3;
        }
        switch (activeLeftLayer)
        {
            case 0:
                //for (int i = 0; i < leftLayer1.Count; i++)
                //{
                //    leftLayer4[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = leftLayer2;
                spriteRenderers = leftLayer1;
                break;
            case 1:
                //for (int i = 0; i < leftLayer2.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(true);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = leftLayer3;
                spriteRenderers = leftLayer2;
                break;
            case 2:
                //for (int i = 0; i < leftLayer3.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(true);
                //    leftLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = leftLayer4;
                spriteRenderers = leftLayer3;
                break;
            case 3:
                //for (int i = 0; i < leftLayer4.Count; i++)
                //{
                //    leftLayer1[i].gameObject.SetActive(false);
                //    leftLayer2[i].gameObject.SetActive(false);
                //    leftLayer3[i].gameObject.SetActive(false);
                //    leftLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = leftLayer1;
                spriteRenderers = leftLayer4;
                break;

        }
        _selectedFace = _cubeState.left;
        _cubeSelectFace.RotateSide(_cubeState.left, -1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void DecreaseRightLayer(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeRightLayer--;
        if (activeRightLayer < 0)
        {
            activeRightLayer = 3;
        }
        switch (activeRightLayer)
        {
            case 0:
                //for (int i = 0; i < rightLayer1.Count; i++)
                //{
                //    rightLayer4[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer1[i].gameObject.SetActive(true);
                //}
                OldSprites = rightLayer2;
                spriteRenderers = rightLayer1;
                break;
            case 1:
                //for (int i = 0; i < rightLayer2.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(true);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = rightLayer3;
                spriteRenderers = rightLayer2;
                break;
            case 2:
                //for (int i = 0; i < rightLayer3.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(true);
                //    rightLayer4[i].gameObject.SetActive(false);
                //}
                OldSprites = rightLayer4;
                spriteRenderers = rightLayer3;
                break;
            case 3:
                //for (int i = 0; i < rightLayer4.Count; i++)
                //{
                //    rightLayer1[i].gameObject.SetActive(false);
                //    rightLayer2[i].gameObject.SetActive(false);
                //    rightLayer3[i].gameObject.SetActive(false);
                //    rightLayer4[i].gameObject.SetActive(true);
                //}
                OldSprites = rightLayer1;
                spriteRenderers = rightLayer4;
                break;

        }
        _selectedFace = _cubeState.right;
        _cubeSelectFace.RotateSide(_cubeState.right, -1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void DecreaseCenterVertical(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeCenterVerticalLayer--;
        if (activeCenterVerticalLayer < 0)
        {
            activeCenterVerticalLayer = 3;
        }
        switch (activeCenterVerticalLayer)
        {
            case 0:
                //for (int i = 0; i < centerVertical1.Count; i++)
                //{
                //    centerVertical4[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical1[i].gameObject.SetActive(true);
                //}
                OldSprites = centerVertical2;
                spriteRenderers = centerVertical1;
                break;
            case 1:
                //for (int i = 0; i < centerVertical2.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(true);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerVertical3;
                spriteRenderers = centerVertical2;
                break;
            case 2:
                //for (int i = 0; i < centerVertical3.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(true);
                //    centerVertical4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerVertical4;
                spriteRenderers = centerVertical3;
                break;
            case 3:
                //for (int i = 0; i < centerVertical4.Count; i++)
                //{
                //    centerVertical1[i].gameObject.SetActive(false);
                //    centerVertical2[i].gameObject.SetActive(false);
                //    centerVertical3[i].gameObject.SetActive(false);
                //    centerVertical4[i].gameObject.SetActive(true);
                //}
                OldSprites = centerVertical1;
                spriteRenderers = centerVertical4;
                break;

        }
        _selectedFace = _cubeState.centerVertical;
        _cubeSelectFace.RotateSide(_cubeState.centerVertical, 1);
        StartCoroutine(InvokeRubicMoved());
    }
    public void DecreaseCenterHorizontal(Button button)
    {
        if (!button.interactable)
        {
            return;
        }
        StartCoolDown?.Invoke();
        _readCube.ReadState();
        activeCenterHorizontalLayer--;
        if (activeCenterHorizontalLayer < 0)
        {
            activeCenterHorizontalLayer = 3;
        }
        switch (activeCenterHorizontalLayer)
        {
            case 0:
                //for (int i = 0; i < centerHorizontal1.Count; i++)
                //{
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal1[i].gameObject.SetActive(true);
                //}
                OldSprites = centerHorizontal2;
                spriteRenderers = centerHorizontal1;
                break;
            case 1:
                //for (int i = 0; i < centerHorizontal2.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(true);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerHorizontal3; //Old sprites are which were active before move
                spriteRenderers = centerHorizontal2;
                break;
            case 2:
                //for (int i = 0; i < centerHorizontal3.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(true);
                //    centerHorizontal4[i].gameObject.SetActive(false);
                //}
                OldSprites = centerHorizontal4;
                spriteRenderers = centerHorizontal3;
                break;
            case 3:
                //for (int i = 0; i < centerHorizontal4.Count; i++)
                //{
                //    centerHorizontal1[i].gameObject.SetActive(false);
                //    centerHorizontal2[i].gameObject.SetActive(false);
                //    centerHorizontal3[i].gameObject.SetActive(false);
                //    centerHorizontal4[i].gameObject.SetActive(true);
                //}
                OldSprites = centerHorizontal1;
                spriteRenderers = centerHorizontal4;
                break;

        }
        _selectedFace = _cubeState.centerHorizontal;
        _cubeSelectFace.RotateSide(_cubeState.centerHorizontal, -1);
        StartCoroutine(InvokeRubicMoved());
    }

    private void InitiateLayers()
    {
        AddToLayerList(upLayer2, CubeUpLayer2);
        AddToLayerList(upLayer3, CubeUpLayer3);
        AddToLayerList(upLayer4, CubeUpLayer4);
        AddToLayerList(downLayer2, CubeDownLayer2);
        AddToLayerList(downLayer3, CubeDownLayer3);
        AddToLayerList(downLayer4, CubeDownLayer4);
        AddToLayerList(leftLayer2, CubeLeftLayer2);
        AddToLayerList(leftLayer3, CubeLeftLayer3);
        AddToLayerList(leftLayer4, CubeLeftLayer4);
        AddToLayerList(rightLayer2, CubeRightLayer2);
        AddToLayerList(rightLayer3, CubeRightLayer3);
        AddToLayerList(rightLayer4, CubeRightLayer4);
        AddToLayerList(centerHorizontal2, CubeCenterHorizontal2);
        AddToLayerList(centerHorizontal3, CubeCenterHorizontal3);
        AddToLayerList(centerHorizontal4, CubeCenterHorizontal4);
        AddToLayerList(centerVertical2, CubeCenterVertical2);
        AddToLayerList(centerVertical3, CubeCenterVertical3);
        AddToLayerList(centerVertical4, CubeCenterVertical4);
    }

    public void HideRubic()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void AddToLayerList(List<SpriteRenderer> addToSlotList, List<SpriteRenderer> addToCubeList)
    {
        for (int i = 0; i < 3; i++)
        {
            //Just assign sprites It doesn't needed to setActive()
            int random = UnityEngine.Random.Range(0, spriteCollection.Count - 5 + GameController.Instance._current_bet_index);
            addToSlotList[i].sprite = imagesStrings[random].sprite;
            addToCubeList[i].sprite = imagesStrings[random].sprite;
            addToCubeList[i].GetComponent<Icons>()._sprite = imagesStrings[random].sprite;
            addToSlotList[i].gameObject.name = imagesStrings[random].name;

            //set the name so that winning condition can be satisfied

            //addToList[i].transform.gameObject.SetActive(true);
        }
    }
    public void DetectSprite()
    {

        for (int i = 0; i < originTransforms.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(originTransforms[i].position, Vector3.forward);
            if (hit.transform != null)
            {
                frontSprites[i].sprite = hit.transform.GetComponent<Icons>().GetSprite();
                CubeFrontFace[i].sprite = hit.transform.GetComponent<Icons>().GetSprite();
                CubeFrontFace[i].transform.GetComponent<Icons>()._sprite = hit.transform.GetComponent<Icons>().GetSprite();
                frontSprites[i].name = hit.transform.gameObject.name;
                hit.transform.gameObject.SetActive(false);
                frontSprites[i].gameObject.SetActive(true);
                _detected.Add(hit.transform.GetComponent<SpriteRenderer>());
            }
        }
        InitiateLayers();
    }

}
