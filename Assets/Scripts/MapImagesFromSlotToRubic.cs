using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
internal class SpriteCounting
{
    public Sprite sprite;
    public int totalAppearences;
}
public class MapImagesFromSlotToRubic : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> frontSprites = new();
    [SerializeField] private List<Transform> raycastOrigins = new();

    [SerializeField] private List<SpriteCounting> icons = new();
    [SerializeField] private List<SpriteRenderer> cubeSprites = new();  //This list contains sprites other than front face sprites

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(DetectFromSlot), 3f);
        Invoke(nameof(InitiateCube), 3f);
    }

    private void DetectFromSlot()
    {
        for (int i = 0; i < raycastOrigins.Count; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigins[i].position, Vector3.forward);
            if (hit.transform != null)
            {
                Debug.Log("Worked");
                frontSprites[i].sprite = hit.transform.GetComponent<Icons>().GetSprite();

            }
        }
    }

    private void InitiateCube()
    {
        for (int i = 0; i < cubeSprites.Count; i++)
        {
            int random = Random.Range(0, icons.Count);
            cubeSprites[i].sprite = icons[random].sprite;
        }
    }
}
