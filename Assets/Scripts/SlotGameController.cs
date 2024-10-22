//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(SlotGameController))]
//public class SlotGameControllerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        SlotGameController slotGameController = (SlotGameController)target;

//        // Draw the default inspector
//        DrawDefaultInspector();

//        GUILayout.Space(10);

//        // Add a button to manually update the grid
//        if (GUILayout.Button("Update Grid Manually"))
//        {
//            slotGameController.UpdateGridManually();
//        }
//    }
//}

//public class SlotGameController : MonoBehaviour
//{
//    // Define the size of the grid
//    private const int numRows = 4;
//    private const int numCols = 5;

//    // 2D array to represent the grid
//    private int[,] gameGrid = new int[numRows, numCols];

//    // Define the position and size of the grid in the editor
//    public Vector2 gridPosition = new Vector2(0, 0);
//    public float cellWidth = 1.0f;
//    public float cellHeight = 1.0f;

//    void Start()
//    {
//        // Example: Initialize the grid with random slot images for demonstration
//        InitializeGrid();
//    }

//    void InitializeGrid()
//    {
//        for (int i = 0; i < numRows; i++)
//        {
//            for (int j = 0; j < numCols; j++)
//            {
//                // Assign a random slot image ID (replace this with your logic)
//                gameGrid[i, j] = Random.Range(1, 4);
//            }
//        }
//    }

//    bool CheckForWin()
//    {
//        for (int i = 0; i < numRows; i++)
//        {
//            if (CheckRow(i))
//                return true;
//        }

//        for (int j = 0; j < numCols; j++)
//        {
//            if (CheckColumn(j))
//                return true;
//        }

//        if (CheckDiagonal() || CheckAntiDiagonal())
//            return true;

//        return false;
//    }

//    bool CheckRow(int rowIndex)
//    {
//        int firstImage = gameGrid[rowIndex, 0];

//        if (firstImage == 0)
//            return false;

//        for (int j = 1; j < numCols; j++)
//        {
//            if (gameGrid[rowIndex, j] != firstImage)
//                return false;
//        }

//        return true;
//    }

//    bool CheckColumn(int colIndex)
//    {
//        int firstImage = gameGrid[0, colIndex];

//        if (firstImage == 0)
//            return false;

//        for (int i = 1; i < numRows; i++)
//        {
//            if (gameGrid[i, colIndex] != firstImage)
//                return false;
//        }

//        return true;
//    }

//    bool CheckDiagonal()
//    {
//        int firstImage = gameGrid[0, 0];

//        if (firstImage == 0)
//            return false;

//        for (int i = 1; i < Mathf.Min(numRows, numCols); i++)
//        {
//            if (gameGrid[i, i] != firstImage)
//                return false;
//        }

//        return true;
//    }

//    bool CheckAntiDiagonal()
//    {
//        int firstImage = gameGrid[0, numCols - 1];

//        if (firstImage == 0)
//            return false;

//        for (int i = 1; i < Mathf.Min(numRows, numCols); i++)
//        {
//            if (gameGrid[i, numCols - 1 - i] != firstImage)
//                return false;
//        }

//        return true;
//    }

//    // Example usage in your game logic
//    void Update()
//    {
//        if (CheckForWin())
//        {
//            // Handle win condition
//            Debug.Log("You won!");
//        }
//    }

//    // Draw grid and display cell content in the editor
//    void OnDrawGizmos()
//    {
//        Handles.color = Color.white;

//        for (int i = 0; i < numRows; i++)
//        {
//            for (int j = 0; j < numCols; j++)
//            {
//                Vector3 cellPosition = new Vector3(gridPosition.x + j * cellWidth, gridPosition.y - i * cellHeight, 0.0f);

//                // Draw grid lines
//                Handles.DrawLine(cellPosition, cellPosition + Vector3.right * cellWidth);
//                Handles.DrawLine(cellPosition, cellPosition - Vector3.up * cellHeight);

//                // Display cell content
//                GUIStyle style = new GUIStyle();
//                style.normal.textColor = Color.white;
//                style.alignment = TextAnchor.MiddleCenter;
//                Handles.Label(cellPosition + new Vector3(cellWidth, -cellHeight, 0.0f) * 0.5f, gameGrid[i, j].ToString(), style);
//            }
//        }

//        // Draw the last row and column
//        Handles.DrawLine(new Vector3(gridPosition.x, gridPosition.y - numRows * cellHeight, 0), new Vector3(gridPosition.x + numCols * cellWidth, gridPosition.y - numRows * cellHeight, 0));
//        Handles.DrawLine(new Vector3(gridPosition.x + numCols * cellWidth, gridPosition.y, 0), new Vector3(gridPosition.x + numCols * cellWidth, gridPosition.y - numRows * cellHeight, 0));
//    }

//    // Function to update the grid manually from the inspector
//    public void UpdateGridManually()
//    {
//        InitializeGrid();
//    }
//}
