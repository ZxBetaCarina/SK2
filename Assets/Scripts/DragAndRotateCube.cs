using UnityEngine;

public class DrabAndRotateCube : MonoBehaviour
{
    private Vector3 mouseStartPosition;
    private Vector3 rotationStart;
    private bool isDragging;
    [SerializeField] private Transform _rubicCube;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void StartDrag()
    {
        // Get the mouse position at the start of the drag
        mouseStartPosition = Input.mousePosition;
        rotationStart = _rubicCube.rotation.eulerAngles;
        isDragging = true;
    }

    void ContinueDrag()
    {
        if (isDragging)
        {
            // Calculate the rotation amount based on mouse movement
            Vector3 mouseDelta = Input.mousePosition - mouseStartPosition;
            float rotationX = mouseDelta.y * 0.1f;
            float rotationY = -mouseDelta.x * 0.1f;

            // Apply the rotation to the cube
            _rubicCube.rotation = Quaternion.Euler(rotationStart.x + rotationX, rotationStart.y + rotationY, rotationStart.z);
        }
    }

    void EndDrag()
    {
        isDragging = false;
    }
}