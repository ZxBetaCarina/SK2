using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    public float targetAspect = 9f / 16f; // Set this to your desired aspect ratio (e.g., 9:16 for portrait)

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        UpdateLetterbox();
    }

    void UpdateLetterbox()
    {
        // Determine the game window's current aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // Current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / targetAspect;

        // If scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else // Add pillarbox
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }

    void OnPreCull()
    {
        // Ensure the letterbox updates if the screen is resized
        UpdateLetterbox();
    }
}