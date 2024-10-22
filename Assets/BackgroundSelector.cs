using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSelector : MonoBehaviour
{
    // Array to hold the possible background sprites
    public Sprite[] backgrounds;

    // Reference to the background Image component
    public Image backgroundImage;

    // This function is called when the game starts
    void Start()
    {
        // Call the function to change the background
        ChangeBackground();
    }

    // Function to randomly change the background
    void ChangeBackground()
    {
        if (backgrounds.Length == 0 || backgroundImage == null)
        {
            Debug.LogError("Backgrounds array or Image component not set!");
            return;
        }

        // Pick a random index from the backgrounds array
        int randomIndex = Random.Range(0, backgrounds.Length);

        // Set the background to the randomly selected sprite
        backgroundImage.sprite = backgrounds[randomIndex];
    }
}
