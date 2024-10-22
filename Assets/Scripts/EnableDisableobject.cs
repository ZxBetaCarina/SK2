using UnityEngine;

public class EnableDisableObject : MonoBehaviour
{
    public GameObject NormalPaytable;
    public GameObject FollowPaytable;
    //public Button toggleButton;
    private bool isFollowObjectActive = false; // assuming the object is initially active
    private bool isNormalObjectActive = false; // assuming the object is initially active


    void Start()
    {
        //toggleButton.onClick.AddListener(ToggleObject);
    }

    public void NormalToggleObject()
    {
        isNormalObjectActive = !isNormalObjectActive; // Toggle the state
        NormalPaytable.SetActive(isNormalObjectActive);
    }
    public void FollowToggleObject()
    {
        isFollowObjectActive = !isFollowObjectActive; // Toggle the state
        FollowPaytable.SetActive(isFollowObjectActive);
    }
}
