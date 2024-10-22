using System.Collections.Generic;
using UnityEngine;

public class RubicMovement : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public List<Vector3> spritePositions;
    public List<GameObject> newSpriteObjects;
    public Transform originalParent;
    public Transform parentTransform;
    public float rotationSpeed = 30f; // Adjust this value as needed
    public float targetRotation = -90f; // Target rotation angle
    private bool shouldRotate = false;

    void Start()
    {
        // Initialize the arrays and lists
        spritePositions = new List<Vector3>();
        newSpriteObjects = new List<GameObject>();


        // Assuming you have SpriteRenderer components attached to GameObjects in the scene
        // You can assign them like this
        //spriteRenderers[0] = GameObject.Find("Sprite1").GetComponent<SpriteRenderer>();
        //spriteRenderers[1] = GameObject.Find("Sprite2").GetComponent<SpriteRenderer>();
        //spriteRenderers[2] = GameObject.Find("Sprite3").GetComponent<SpriteRenderer>();

        // Now you can access and manipulate the sprite renderers in the array as needed
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            // Add the world position of each sprite renderer to the list
            spritePositions.Add(sr.transform.position);
            sr.transform.SetParent(parentTransform);
        }

        // Rotate the parent GameObject to -90 degrees around the Y axis
        parentTransform.rotation = Quaternion.Euler(0f, 90f, 0f);

        // Create new GameObjects with SpriteRenderers at the stored positions
        foreach (Vector3 pos in spritePositions)
        {
            GameObject newSpriteObject = new GameObject("NewSprite");
            newSpriteObject.transform.position = pos;
            newSpriteObject.transform.localScale = Vector3.one * 0.15f;
            SpriteRenderer newSpriteRenderer = newSpriteObject.AddComponent<SpriteRenderer>();
            newSpriteRenderer.sprite = spriteRenderers[0].sprite; // You can assign any sprite you want here
            newSpriteObject.transform.SetParent(parentTransform); // Set the parent
            newSpriteObjects.Add(newSpriteObject);
        }
        shouldRotate = true;
    }

    void Update()
    {
        if (shouldRotate)
        {
            // Rotate the parent GameObject with the specified rotation speed
            parentTransform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
        // Check if the parent GameObject has reached the target rotation
        if (Mathf.Abs(parentTransform.rotation.eulerAngles.y - targetRotation) < 0.5f)
        {
            shouldRotate = false;
            parentTransform.eulerAngles = new Vector3(0, targetRotation, 0);
            DestroyNewSprites();
        }
    }

    void DestroyNewSprites()
    {
        // Destroy the newly created sprites
        foreach (GameObject obj in newSpriteObjects)
        {
            Destroy(obj);
        }

        // Set the parent of the original sprites back to what it was before
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.transform.SetParent(originalParent);
        }
    }
}
