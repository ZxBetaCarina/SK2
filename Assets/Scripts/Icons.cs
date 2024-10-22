using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Icons : MonoBehaviour
{
    [SerializeField] public Sprite _sprite;

    private void Start()
    {
        GrabSprite();
    }
    public Sprite GetSprite()
    {
        return _sprite;
    }

    private void GrabSprite()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }

}
