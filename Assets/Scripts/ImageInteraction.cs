//using UnityEngine;

//public class ImageInteraction : MonoBehaviour
//{
//    public string symbolImageTag = "SymbolImage";
//    public string centerImageTag = "centerpoint";

//    void Update()
//    {
//        // Find GameObjects with the specified tags
//        GameObject symbolImage = GameObject.FindGameObjectWithTag(symbolImageTag);
//        GameObject centerImage = GameObject.FindGameObjectWithTag(centerImageTag);

//        // Check if both images are found
//        if (symbolImage != null && centerImage != null)
//        {
//            // Get the position of the symbolImage
//            Vector3 symbolImagePosition = symbolImage.transform.position;

//            // Get the position of the centerImage
//            Vector3 centerImagePosition = centerImage.transform.position;

//            // Calculate the distance between the two images
//            float distance = Vector3.Distance(symbolImagePosition, centerImagePosition);

//            // Check if the symbolImage is touching the centerImage (you can adjust the threshold)
//            if (distance < 1f)
//            {
//                // Debug information
//                Debug.Log("SymbolImage is touching CenterImage!");
//            }
//        }
//        else
//        {
//            // Debug information if one or both images are not found
//            Debug.LogWarning("One or both images not found!");
//        }
//    }
//}
