using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("Balance", 10);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeScene()
    {
        Debug.Log("CurrentBetIndex 2 "+PlayerPrefs.GetFloat("CurrentBetIndex")); 
        SceneManager.LoadScene(1);
    }
}
