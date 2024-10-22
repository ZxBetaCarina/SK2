using System.Collections.Generic;
using UnityEngine;

public class CubeState : MonoBehaviour
{
    // sides
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();
    public List<GameObject> centerVertical = new();
    public List<GameObject> centerHorizontal = new();

    public static bool autoRotating = false;
    public static bool started = false;
    private Transform centerParent;

    private void OnEnable()
    {
        SelectFace.RotateCenterRow += PickUpParent;
    }
    private void OnDisable()
    {
        SelectFace.RotateCenterRow -= PickUpParent;
    }
    public void PickUp(List<GameObject> cubeSide)
    {
        foreach (GameObject face in cubeSide)
        {
            // Attach the parent of each face (the little cube)
            // to the parent of the 4th index (the little cube in the middle) 
            // Unless it is already the 4th index
            face.transform.parent.transform.parent = centerParent;
            //if (cubeSide == centerHorizontal)
            //{
            //    Debug.Log("Horizontal");
            //    //seperate logic for centerhorizontal
            //    face.transform.parent.transform.parent = centerParent;
            //}
            //else if (cubeSide == centerVertical)
            //{
            //    Debug.Log("Vertical");
            //    //seperate logic for centervertical
            //    face.transform.parent.transform.parent = centerParent;
            //}
            //else if (face != cubeSide[4])
            //{
            //    face.transform.parent.transform.parent = cubeSide[4].transform.parent;
            //}
        }

    }

    private void PickUpParent(Transform parent)
    {
        centerParent = parent;
        Debug.Log("picked");
    }

    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject littleCube in littleCubes)
        {
            if (littleCube != littleCubes[4])
            {
                littleCube.transform.parent.transform.parent = pivot;
            }
        }
    }

    string GetSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach (GameObject face in side)
        {
            sideString += face.name[0].ToString();
        }
        return sideString;
    }

    public string GetStateString()
    {
        string stateString = "";
        stateString += GetSideString(up);
        stateString += GetSideString(right);
        stateString += GetSideString(front);
        stateString += GetSideString(down);
        stateString += GetSideString(left);
        stateString += GetSideString(back);
        return stateString;
    }
}
