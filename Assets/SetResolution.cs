using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    public void HandleResolution(int val)
    {
        if (val == 0)
        {
            SetResolution(2160,3840);
        }
        if (val == 1)
        {
            SetResolution(768, 1366);
        }
        if(val == 2)
        {
            SetResolution(564, 960);
        }
    }

    void SetResolution(int p_width, int p_height)
    {
#if UNITY_STANDALONE
        Screen.SetResolution(p_width, p_height, false);
        Screen.fullScreen = false;
#endif
    }
}