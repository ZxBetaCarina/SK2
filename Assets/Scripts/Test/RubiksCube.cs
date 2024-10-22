using UnityEngine;

public class RubiksCube : MonoBehaviour
{
    // Set the rotation speed in the Unity Editor
    public float rotationSpeed = 90f; // You can adjust the speed

    // Enum to represent the rotation direction
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
        HalfTurn
    }

    // Function to rotate the specified layer of the Rubik's Cube
    public void RotateLayer(string axis, int layerIndex, RotationDirection direction)
    {
        Transform[] layerToRotate = GetLayer(axis, layerIndex);

        if (layerToRotate != null)
        {
            // Calculate the rotation amount based on the speed and frame rate
            float rotationAmount = rotationSpeed * Time.deltaTime;

            // Adjust the rotation amount based on the direction
            switch (direction)
            {
                case RotationDirection.CounterClockwise:
                    rotationAmount = -rotationAmount;
                    break;
                case RotationDirection.HalfTurn:
                    rotationAmount *= 2f;
                    break;
            }

            // Rotate each cubelet in the specified layer around the axis
            foreach (Transform cubelet in layerToRotate)
            {
                cubelet.Rotate(axis == "X" ? Vector3.right : (axis == "Y" ? Vector3.up : Vector3.forward), rotationAmount);
            }
        }
        else
        {
            Debug.LogWarning("Layer not found: " + axis + layerIndex);
        }
    }

    // Function to dynamically detect and rotate layers based on the Rubik's Cube configuration
    public void RotateDynamic(string axis, int layerIndex, RotationDirection direction)
    {
        // Check if layers need to be dynamically detected
        if (layerIndex < 0)
        {
            DetectLayers(axis);
            return;
        }

        // Rotate the specified layer
        RotateLayer(axis, layerIndex, direction);
    }

    // Function to detect the layers of the Rubik's Cube dynamically
    private void DetectLayers(string axis)
    {
        // Dynamic detection logic goes here
        // You need to analyze the positions of cubelets and determine the layers dynamically

        Debug.Log("Dynamic layer detection is not implemented yet!");
    }

    // Function to get the transforms of the cubes in a specified layer
    private Transform[] GetLayer(string axis, int layerIndex)
    {
        Transform[] layer = new Transform[9]; // Assuming a 3x3x3 Rubik's Cube

        // Iterate through the cubes and find the ones in the specified layer
        int cubeIndex = 0;
        foreach (Transform cubelet in transform)
        {
            float positionOnAxis = axis == "X" ? cubelet.localPosition.x : (axis == "Y" ? cubelet.localPosition.y : cubelet.localPosition.z);
            if (Mathf.Round(positionOnAxis) == layerIndex)
            {
                layer[cubeIndex] = cubelet;
                cubeIndex++;
            }
        }

        // Debug information
        Debug.Log("Layer: " + axis + layerIndex + " | Found cubes: " + cubeIndex);

        // Check if the correct number of cubes was found
        if (cubeIndex == 9)
        {
            return layer;
        }
        else
        {
            return null;
        }
    }
}
