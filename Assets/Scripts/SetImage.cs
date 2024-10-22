using UnityEngine;
using UnityEngine.UI;

public class SetImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Image>().sprite = transform.GetComponent<Icons>().GetSprite();
    }
}
