using UnityEngine;

public class DualParent : MonoBehaviour
{
    public GameObject parentObjectA;
    public GameObject parentObjectB;
    public GameObject commonChildObject;

    void Start()
    {
        // Check if all the necessary objects are assigned
        if (parentObjectA != null && parentObjectB != null && commonChildObject != null)
        {
            // Set commonChildObject as a child of parentObjectA
            commonChildObject.transform.parent = parentObjectA.transform;

            // Now create a new empty GameObject for parentObjectB
            GameObject emptyParentB = new GameObject("EmptyParentB");
            parentObjectB.transform.parent = emptyParentB.transform;

            // Set commonChildObject as a child of parentObjectB
            commonChildObject.transform.parent = emptyParentB.transform;
        }
        else
        {
            Debug.LogError("Please assign all GameObjects in the inspector.");
        }
    }
}
