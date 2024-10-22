//using System.Collections;
//using UnityEngine;

//public class CubeManager : MonoBehaviour
//{
//    public GameObject cubePrefab;
//    public int cubeCount = 3; // Adjust this value for different cube sizes

//    void Start()
//    {
//        GenerateRubiksCube();
//    }

//    void GenerateRubiksCube()
//    {
//        for (int x = 0; x < cubeCount; x++)
//        {
//            for (int y = 0; y < cubeCount; y++)
//            {
//                for (int z = 0; z < cubeCount; z++)
//                {
//                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
//                    cube.transform.parent = transform; // Set the CubeManager as the parent
//                }
//            }
//        }
//    }
//}
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject cubePrefab;
    public int cubeCount = 3; // Adjust this value for different cube sizes

    void Start()
    {
        GenerateRubiksCube();
    }

    void GenerateRubiksCube()
    {
        for (int x = 0; x < cubeCount; x++)
        {
            for (int y = 0; y < cubeCount; y++)
            {
                for (int z = 0; z < cubeCount; z++)
                {
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                    cube.transform.parent = transform; // Set the CubeManager as the parent

                    // Apply a different color to each cube
                    cube.GetComponent<Renderer>().material.color = GetRandomColor();
                }
            }
        }
    }

    Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
