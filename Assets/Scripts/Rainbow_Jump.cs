using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Rainbow_Jump : MonoBehaviour
{
    public Button Rainbowjump;
    // Start is called before the first frame update
    void Start()
    {
        Rainbowjump.onClick.AddListener(OnRainbowjumpClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRainbowjumpClick()
    {
        SceneManager.LoadScene(2);
    }
}
