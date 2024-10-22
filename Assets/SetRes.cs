using UnityEngine;

public class SetRes : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //Set screen size for Standalone
#if UNITY_STANDALONE
        Screen.SetResolution(564, 960, false);
        Screen.fullScreen = false;
#endif

    }

    private void Update()
    {
        
    }

}
