using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject gridTilePrefab;
    [SerializeField] private GameObject tiles;
    [SerializeField] private GameObject grid;

    [Header("Settings")]
    [SerializeField] private float tileSpacing;
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    private GameObject[,] board;

    enum Directions
    {
        Up, Down, Left, Right
    }

    void MoveTile(int row, int column, Directions direction)
    {
        GameObject tileObject = board[row, column];

        if (tileObject == null)
        {
            return;
        }

        Vector2Int directionVector = new Vector2Int(0, 0);
        switch (direction)
        {
            case Directions.Up:
                directionVector = new Vector2Int(-1, 0);
                break;

            case Directions.Down:
                directionVector = new Vector2Int(1, 0);
                break;

            case Directions.Right:
                directionVector = new Vector2Int(0, 1);
                break;

            case Directions.Left:
                directionVector = new Vector2Int(0, -1);
                break;
        }


        Vector2Int position = new Vector2Int(row, column);

        while (true)
        {
            Vector2Int nextPosition = position + directionVector;

            bool isNextPositionValid = true;

            // check for out of bounds
            if (nextPosition.x > rows - 1 || nextPosition.x < 0 || nextPosition.y > columns - 1 || nextPosition.y < 0)
            {
                Debug.Log("out of bounds");
                isNextPositionValid = false;
            }

            if (isNextPositionValid)
            {
                // check if position is occupied
                if (board[nextPosition.x, nextPosition.y] != null)
                {
                    Debug.Log("occupied");
                    isNextPositionValid = false;

                    // check for merging
                    if (board[nextPosition.x, nextPosition.y].GetComponent<Tile>().Value == tileObject.GetComponent<Tile>().Value
                    && ! board[nextPosition.x, nextPosition.y].GetComponent<Tile>().JustMerged)
                    {
                        board[nextPosition.x, nextPosition.y].GetComponent<Tile>().NewValue = board[nextPosition.x, nextPosition.y].GetComponent<Tile>().Value * 2;
                        board[nextPosition.x, nextPosition.y].GetComponent<Tile>().JustMerged = true;

                        board[nextPosition.x, nextPosition.y].GetComponent<Tile>().MovingToLabelAlpha = 0f;

                        tileObject.GetComponent<Tile>().MovingToGameObject = board[nextPosition.x, nextPosition.y];
                        tileObject.transform.localPosition = new Vector3(tileObject.transform.localPosition.x, tileObject.transform.localPosition.y, 5);
                        tileObject.GetComponent<Tile>().MovingToLabelAlpha = 0f;
                        tileObject.GetComponent<Tile>().scheduleDestruction = true;

                        board[position.x, position.y] = null;

                        return;
                    }
                }
            }
            
            

            if (isNextPositionValid)
            {
                board[position.x, position.y] = null;
                position = nextPosition;
            }
            else
            {
                board[row, column] = null;
                board[position.x, position.y] = tileObject;
                tileObject.GetComponent<Tile>().MovingToPosition = new Vector3(position.y * tileSpacing, position.x * -tileSpacing, 0);
                Debug.Log(position - new Vector2Int(row, column));
                return;
            }
        }
    }

    void AddRandomTile()
    {
        for (int i = 0; i < 500; i++)
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(0, rows), Random.Range(0, columns));
            if (board[randomPosition.x, randomPosition.y] == null)
            {
                CreateTile(randomPosition.x, randomPosition.y, Random.Range(1, 3) * 2);
                return;
            }
        }
        Debug.Log("You loose");
    }
    void MoveBoard(Directions direction)
    {
        switch (direction)
        {
            case Directions.Down:
                for (int row = rows - 1; row >= 0; row--)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        MoveTile(row, column, direction);
                    }
                }
                break;
            case Directions.Up:
                for (int row = 0; row < rows; row++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        MoveTile(row, column, direction);
                    }
                }
                break;
            case Directions.Left:
                for (int column = 0; column < columns; column++)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        MoveTile(row, column, direction);
                    }
                }
                break;
            case Directions.Right:
                for (int column = columns-1; column >= 0; column--)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        MoveTile(row, column, direction);
                    }
                }
                break;
        }
        AddRandomTile();
    }
    GameObject CreateTile(int row, int column, int value)
    {
        GameObject newTile = Instantiate(tilePrefab, tiles.transform);
        board[row, column] = newTile;
        newTile.transform.localPosition = new Vector3(column * tileSpacing, row * -tileSpacing, 0);
        newTile.GetComponent<Tile>().MovingToPosition = newTile.transform.localPosition;
        newTile.GetComponent<Tile>().Value = value;

        return newTile;
    }

    void CreateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newGridTile = Instantiate(gridTilePrefab, grid.transform);
                newGridTile.transform.localPosition = new Vector3(column * tileSpacing, row * -tileSpacing, 0);
            }
        }
    }
    void Start()
    {
        board = new GameObject[rows, columns];
        CreateGrid();

        CreateTile(0, 0, 2);
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("up");
            MoveBoard(Directions.Up);
        }
    }
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("down");
            MoveBoard(Directions.Down);
        }
    }
    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("left");
            MoveBoard(Directions.Left);
        }
    }
    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("right");
            MoveBoard(Directions.Right);
        }
    }
}
