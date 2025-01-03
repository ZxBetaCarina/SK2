﻿using System.Collections.Generic;
using UnityEngine;

public class ReadCube : MonoBehaviour
{
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    private List<GameObject> frontRays = new List<GameObject>();
    private List<GameObject> backRays = new List<GameObject>();
    private List<GameObject> upRays = new List<GameObject>();
    private List<GameObject> downRays = new List<GameObject>();
    private List<GameObject> leftRays = new List<GameObject>();
    private List<GameObject> rightRays = new List<GameObject>();
    [SerializeField]
    private List<GameObject> centerVerticalRays = new List<GameObject>();
    [SerializeField]
    private List<GameObject> centerHorizontalRays = new List<GameObject>();

    private int layerMask = 1 << 8; // this layerMask is for the faces of the cube only
    CubeState cubeState;
    CubeMap cubeMap;
    public GameObject emptyGO;
    private bool leftOrRight = false;
    private bool upOrDown = false;

    // Start is called before the first frame update
    void Start()
    {
        SetRayTransforms();


        cubeState = FindObjectOfType<CubeState>();
        cubeMap = FindObjectOfType<CubeMap>();
        Invoke(nameof(ReadState), .2f);
        CubeState.started = true;


    }

    public void ReadState()
    {

        // set the state of each position in the list of sides so we know
        // what color is in what position
        leftOrRight = false;
        cubeState.up = ReadFace(upRays, tUp);
        cubeState.down = ReadFace(downRays, tDown);
        cubeState.left = ReadFace(leftRays, tLeft);
        cubeState.right = ReadFace(rightRays, tRight);
        cubeState.front = ReadFace(frontRays, tFront);
        cubeState.back = ReadFace(backRays, tBack);
        cubeState.centerHorizontal = ReadFace(centerHorizontalRays, tBack);
        cubeState.centerVertical = ReadFace(centerVerticalRays, tBack);
        // update the map with the found positions
        //cubeMap.Set();
    }


    void SetRayTransforms()
    {
        // populate the ray lists with raycasts eminating from the transform, angled towards the cube.
        upOrDown = true;
        upRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        upOrDown = false;
        frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
        backRays = BuildRays(tBack, new Vector3(0, 270, 0));
        leftOrRight = true;
        leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
        leftOrRight = false;
    }


    List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
    {
        // The ray count is used to name the rays so we can be sure they are in the right order.
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();
        // This creates 9 rays in the shape of the side of the cube with
        // Ray 0 at the top left and Ray 8 at the bottom right:
        //  |0|1|2|
        //  |3|4|5|
        //  |6|7|8|

        for (int y = 1; y > -2; y--)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3 startPos = new Vector3(rayTransform.position.x + x,
                                                rayTransform.position.y + y,
                                                rayTransform.position.z);
                GameObject rayStart = Instantiate(emptyGO, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;
                if (!leftOrRight)
                {
                    if (x == 0)
                    {
                        centerVerticalRays.Add(rayStart);
                    }
                }
                if (!upOrDown)
                {
                    if (y == 0)
                    {
                        centerHorizontalRays.Add(rayStart);
                    }
                }
            }
        }
        rayTransform.localRotation = Quaternion.Euler(direction);
        return rays;

    }

    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        List<GameObject> facesHit = new List<GameObject>();
        if (rayStarts == centerHorizontalRays)
        {
            foreach (GameObject rayStart in rayStarts)
            {
                Vector3 ray = rayStart.transform.position;
                RaycastHit hit;

                if (Physics.Raycast(ray, Vector3.forward, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.forward * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.back, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.back * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.left, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.left * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.right, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
            }

            return facesHit;
        }
        if (rayStarts == centerVerticalRays)
        {
            foreach (GameObject rayStart in rayStarts)
            {
                Vector3 ray = rayStart.transform.position;
                RaycastHit hit;

                if (Physics.Raycast(ray, Vector3.left, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.left * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.right, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.up, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.up * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                if (Physics.Raycast(ray, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
            }
            return facesHit;
        }

        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 ray = rayStart.transform.position;
            RaycastHit hit;

            // Does the ray intersect any objects in the layerMask?
            if (Physics.Raycast(ray, rayTransform.forward, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
                //print(hit.collider.gameObject.name);
            }
            else
            {
                Debug.DrawRay(ray, rayTransform.forward * 1000, Color.green);
            }
        }
        return facesHit;
    }

}
