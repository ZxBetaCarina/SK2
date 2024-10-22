using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealMechanism : MonoBehaviour
{
    public List<Image> LastRowImageComponent = new();
    private CheckForWinningPatterns checkForWinningPatterns;
    // Start is called before the first frame update
    void Start()
    {
        checkForWinningPatterns = CheckForWinningPatterns.INSTANCE;
    }



    public void RevealHiddenImages()
    {
        for (int i = 0; i < checkForWinningPatterns.ForthRow.Count; i++)
        {
            RaycastHit2D detectedObject = Physics2D.Raycast(checkForWinningPatterns.ForthRow[i].transform.position, Vector3.forward);
            if (detectedObject.transform != null)
            {
                LastRowImageComponent[i].sprite = detectedObject.transform.GetComponent<Icons>().GetSprite();
            }
        }
    }



}
