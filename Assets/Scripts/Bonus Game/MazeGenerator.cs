using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public float cellSize = 1.0f;

    private GameObject mazeParent;
    private int[,] maze;
    private Vector2Int startCell;
    private Vector2Int endCell;

    void Start()
    {
        GenerateMaze();
        InstantiatePlayer();
        InstantiateStartAndEndPoints();
    }

    void GenerateMaze()
    {
        maze = new int[rows, columns];
        startCell = new Vector2Int(0, 0);
        endCell = new Vector2Int(rows - 1, columns - 1);

        // Initialize maze with walls
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                maze[i, j] = 1;
            }
        }

        RecursiveBacktracking(startCell.x, startCell.y);

        // Instantiate maze objects as children of the parent object
        mazeParent = new GameObject("Maze");
        mazeParent.transform.SetParent(transform); // Set parent
        mazeParent.transform.localPosition = Vector3.zero; // Set position to (0,0)

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (maze[i, j] == 1) // Assuming 1 represents a wall
                {
                    Vector3 position = new Vector3(i * cellSize, j * cellSize, 0);
                    GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
                    wall.transform.parent = mazeParent.transform;
                }
            }
        }
    }

    void RecursiveBacktracking(int x, int y)
    {
        maze[x, y] = 0; // Mark cell as visited

        // Create a randomized order of directions (up, down, left, right)
        int[] directions = { 0, 1, 2, 3 };
        Shuffle(directions);

        foreach (int direction in directions)
        {
            int newX = x + (direction == 2 ? -2 : (direction == 3 ? 2 : 0));
            int newY = y + (direction == 0 ? -2 : (direction == 1 ? 2 : 0));

            if (newX >= 0 && newX < rows && newY >= 0 && newY < columns && maze[newX, newY] == 1)
            {
                maze[newX, newY] = 0; // Mark the new cell as visited
                maze[x + (newX - x) / 2, y + (newY - y) / 2] = 0; // Carve path to the new cell

                RecursiveBacktracking(newX, newY);
            }
        }
    }

    void InstantiatePlayer()
    {
        Vector3 playerPosition = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        Instantiate(playerPrefab, playerPosition, Quaternion.identity);
    }

    void InstantiateStartAndEndPoints()
    {
        Vector3 startPointPosition = new Vector3(startCell.x * cellSize, startCell.y * cellSize, 0);
        Instantiate(startPointPrefab, startPointPosition, Quaternion.identity);

        Vector3 endPointPosition = new Vector3(endCell.x * cellSize, endCell.y * cellSize, 0);
        Instantiate(endPointPrefab, endPointPosition, Quaternion.identity);
    }

    // Fisher-Yates shuffle algorithm
    void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Rotate the maze 90 degrees clockwise
    public void RotateClockwise()
    {
        mazeParent.transform.Rotate(Vector3.forward, -90f);
        Debug.Log("Worked");
    }

    // Rotate the maze 90 degrees counterclockwise
    public void RotateCounterClockwise()
    {
        mazeParent.transform.Rotate(Vector3.forward, 90f);
        Debug.Log("Worked");

    }
}

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
