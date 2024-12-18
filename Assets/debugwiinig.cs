using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class debugwiinig : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log("here it is activating "+ this.gameObject.activeSelf);
    }
        
    void OnDisable()
    {
        Debug.Log("here it is activating "+ this.gameObject.activeSelf);
    }
    void Start()
    {
        Debug.Log("here it is activating "+ this.gameObject.activeSelf);
    }
}
