using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RubiksCubeController : MonoBehaviour
{
    public Transform[] cubies; // An array containing all individual cubes
    public float rotationSpeed = 3.0f;
    private bool isRotating = false;

    void Start()
    {
        // Assuming all cubes are direct children of the Rubik's Cube, you can set them in the inspector or dynamically find them:
        cubies = transform.GetComponentsInChildren<Transform>();

        // Attach button click events
        Button rotateLeftButton = GameObject.Find("RotateLeftButton").GetComponent<Button>();
        Button rotateRightButton = GameObject.Find("RotateRightButton").GetComponent<Button>();
        Button rotateUpButton = GameObject.Find("RotateUpButton").GetComponent<Button>();
        Button rotateDownButton = GameObject.Find("RotateDownButton").GetComponent<Button>();

        // Add listeners for button clicks
        rotateLeftButton.onClick.AddListener(() =>
        {
            Debug.Log("Rotate Left Button Clicked");
            StartCoroutine(RotateLayer(Vector3.left, Vector3.back));
        });

        rotateRightButton.onClick.AddListener(() =>
        {
            Debug.Log("Rotate Right Button Clicked");
            StartCoroutine(RotateLayer(Vector3.right, Vector3.forward));
        });

        rotateUpButton.onClick.AddListener(() =>
        {
            Debug.Log("Rotate Up Button Clicked");
            StartCoroutine(RotateLayer(Vector3.up, Vector3.right));
        });

        rotateDownButton.onClick.AddListener(() =>
        {
            Debug.Log("Rotate Down Button Clicked");
            StartCoroutine(RotateLayer(Vector3.down, Vector3.left));
        });
    }

    IEnumerator RotateLayer(Vector3 axis, Vector3 direction)
    {
        if (isRotating)
            yield break;

        isRotating = true;

        Vector3 pivot = CalculatePivot();

        float totalRotation = 90.0f;
        float currentRotation = 0.0f;

        while (currentRotation < totalRotation)
        {
            float rotationAmount = Mathf.Min(rotationSpeed * Time.deltaTime, totalRotation - currentRotation);

            // Rotate each individual cube belonging to the layer around the calculated pivot
            foreach (Transform cubie in cubies)
            {
                if (cubie != transform) // Exclude the Rubik's Cube parent from rotation
                {
                    // Check if the cube is part of the desired layer
                    if (Mathf.Approximately(Vector3.Dot(cubie.position - pivot, axis), 1.0f))
                    {
                        cubie.RotateAround(pivot, direction, rotationAmount);
                    }
                }
            }

            currentRotation += rotationAmount;

            yield return null;
        }

        isRotating = false;
    }

    Vector3 CalculatePivot()
    {
        // Calculate the center of the Rubik's Cube
        Vector3 center = Vector3.zero;

        foreach (Transform cubie in cubies)
        {
            center += cubie.position;
        }

        center /= cubies.Length - 1; // Exclude the Rubik's Cube parent from the calculation

        return center;
    }
}
