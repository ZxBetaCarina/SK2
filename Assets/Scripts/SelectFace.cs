using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectFace : MonoBehaviour
{
    private CubeState cubeState;
    private ReadCube readCube;
    private int layerMask = 1 << 8;
    public static Action<Transform> RotateCenterRow;
    private GameObject centerHorizontal;
    private GameObject centerVertical;
    private GameObject upPivot;
    private GameObject downPivot;
    private GameObject leftPivot;
    private GameObject rightPivot;

    // Start is called before the first frame update
    void Start()
    {
        GameObject newHCenter = new("HorizontalCenter");

        GameObject newVCenter = new("VerticalCenter");

        GameObject newUpPivot = new("UpPivot");

        GameObject newDownPivot = new("DownPivot");

        GameObject newLeftPivot = new("LeftPivot");

        GameObject newRightPivot = new("RightPivot");


        centerHorizontal = Instantiate(newHCenter, transform.position, Quaternion.identity, transform);
        centerHorizontal.transform.AddComponent<PivotRotation>();


        centerVertical = Instantiate(newVCenter, transform.position, Quaternion.identity, transform);
        centerVertical.transform.AddComponent<PivotRotation>();


        upPivot = Instantiate(newUpPivot, transform.position, Quaternion.identity, transform);
        upPivot.transform.AddComponent<PivotRotation>();


        downPivot = Instantiate(newDownPivot, transform.position, Quaternion.identity, transform);
        downPivot.transform.AddComponent<PivotRotation>();


        leftPivot = Instantiate(newLeftPivot, transform.position, Quaternion.identity, transform);
        leftPivot.transform.AddComponent<PivotRotation>();


        rightPivot = Instantiate(newRightPivot, transform.position, Quaternion.identity, transform);
        rightPivot.transform.AddComponent<PivotRotation>();


        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        //readCube.ReadState();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !CubeState.autoRotating)
        {
            // read the current state of the cube            
            //readCube.ReadState();

            // raycast from the mouse towards the cube to see if a face is hit  
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                GameObject face = hit.collider.gameObject;
                // Make a list of all the sides (lists of face GameObjects)
                List<List<GameObject>> cubeSides = new List<List<GameObject>>()
                {
                    cubeState.up,
                    cubeState.down,
                    cubeState.left,
                    cubeState.right,
                    cubeState.front,
                    cubeState.back,
                    cubeState.centerHorizontal,
                    cubeState.centerVertical,
                };
                // If the face hit exists within a side
                foreach (List<GameObject> cubeSide in cubeSides)
                {
                    if (cubeSide.Contains(face))
                    {
                        //Pick it up
                        cubeState.PickUp(cubeSide);
                        //start the side rotation logic
                        //cubeSide[4].transform.parent.GetComponent<PivotRotation>().Rotate(cubeSide);
                    }
                }
            }
        }
    }

    public void RotateSide(List<GameObject> cubeside, int rotateDir)
    {
        //readCube.ReadState();
        if (cubeside == cubeState.centerHorizontal)
        {
            //seperate logic
            RotateCenterRow?.Invoke(centerHorizontal.transform);
            cubeState.PickUp(cubeside);
            centerHorizontal.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;
        }


        if (cubeside == cubeState.centerVertical)
        {
            //seperate logic
            RotateCenterRow?.Invoke(centerVertical.transform);
            cubeState.PickUp(cubeside);
            centerVertical.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;
        }


        if (cubeside == cubeState.left)
        {

            RotateCenterRow?.Invoke(leftPivot.transform);
            cubeState.PickUp(cubeside);
            leftPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;

        }


        if (cubeside == cubeState.right)
        {

            RotateCenterRow?.Invoke(rightPivot.transform);
            cubeState.PickUp(cubeside);
            rightPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;

        }


        if (cubeside == cubeState.up)
        {

            RotateCenterRow?.Invoke(upPivot.transform);
            cubeState.PickUp(cubeside);
            upPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;

        }


        if (cubeside == cubeState.down)
        {

            RotateCenterRow?.Invoke(downPivot.transform);
            cubeState.PickUp(cubeside);
            downPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            return;

        }


    }
}
