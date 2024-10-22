using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateRubic : MonoBehaviour
{
    [SerializeField] private List<Transform> UpFacesRaycasts = new List<Transform>();
    [SerializeField] private List<Transform> DownFacesRaycasts = new List<Transform>();
    [SerializeField] private List<Transform> LeftFacesRaycast = new List<Transform>();
    [SerializeField] private List<Transform> RightFacesRaycast = new List<Transform>();
    [SerializeField] private List<Transform> FrontFacesRaycasts = new List<Transform>();
    [SerializeField] private List<Transform> BackFacesRaycasts = new List<Transform>();


    [SerializeField] private List<GameObject> UpFaces = new List<GameObject>();
    [SerializeField] private List<GameObject> DownFaces = new List<GameObject>();
    [SerializeField] private List<GameObject> LeftFaces = new List<GameObject>();
    [SerializeField] private List<GameObject> RightFaces = new List<GameObject>();
    [SerializeField] private List<GameObject> CenterVertical = new List<GameObject>();
    [SerializeField] private List<GameObject> CenterHorizontal = new List<GameObject>();

    [SerializeField] private Transform _upFacePivot;
    [SerializeField] private Transform _downFacePivot;
    [SerializeField] private Transform _leftFacePivot;
    [SerializeField] private Transform _rightFacePivot;
    [SerializeField] private Transform _centerVerticalFacePivot;
    [SerializeField] private Transform _centerHorizontalFacePivot;
    [SerializeField] private float targetRotation;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UpdateFaceLists());
    }


    private IEnumerator UpdateFaceLists()
    {
        int faces = 0;
        List<GameObject> upFaceList = new List<GameObject>();
        foreach (Transform t in UpFacesRaycasts)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            upFaceList.Add(face.transform.gameObject);
        }
        UpFaces = upFaceList;
        List<GameObject> downFaceList = new List<GameObject>();
        foreach (Transform t in DownFacesRaycasts)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            downFaceList.Add(face.transform.gameObject);
        }
        DownFaces = downFaceList;
        List<GameObject> leftFaceList = new List<GameObject>();
        foreach (Transform t in LeftFacesRaycast)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            leftFaceList.Add(face.transform.gameObject);
        }
        LeftFaces = leftFaceList;
        List<GameObject> rightFaceList = new List<GameObject>();
        foreach (Transform t in RightFacesRaycast)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            rightFaceList.Add(face.transform.gameObject);
        }
        RightFaces = rightFaceList;
        List<GameObject> frontFaceList = new List<GameObject>();
        foreach (Transform t in FrontFacesRaycasts)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            frontFaceList.Add(face.transform.gameObject);
        }

        List<GameObject> backFaceList = new List<GameObject>();
        foreach (Transform t in BackFacesRaycasts)
        {
            RaycastHit2D face = Physics2D.Raycast(t.position, t.forward);
            faces++;
        }
        yield return null;
        Debug.Log(faces);
    }

    public void UpLeft()
    {
        Vector3 target = new Vector3(0, -90, 0);
        foreach (GameObject g in UpFaces)
        {
            g.transform.parent = _upFacePivot;
        }
        _upFacePivot.eulerAngles = _upFacePivot.eulerAngles - target;
        StartCoroutine(nameof(UpdateFaceLists));
    }

    public void UpRight()
    {

    }

    public void DownLeft()
    {

    }

    public void DownRight()
    {

    }

    public void LeftUp()
    {

    }

    public void RightUp()
    {

    }

    public void LeftDown()
    {

    }

    public void RightDown()
    {

    }

    public void CenterUp()
    {

    }

    public void CenterDown()
    {

    }

    public void CenterLeft()
    {

    }

    public void CenterRight()
    {

    }
}
