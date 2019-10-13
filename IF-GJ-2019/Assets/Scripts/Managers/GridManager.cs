using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ui;
using Entities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private Grid grid;
    [Space(10)] [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject firePrefab;

    [Space(10)] [SerializeField] private GameObject cellLitPrefab;
    [SerializeField] private GameObject cellUnlitPrefab;

    [Space(10)] [SerializeField] private CardsManager cardManager;

    [TableMatrix] public Entity[,] matrix;

    public event Action roomFailed;
    public event Action roomCompleted;

    private GameObject root;
    private Vector2Int winCoordinates;

    [Button]
    private void Win()
    {
        roomCompleted?.Invoke();
    }

    [Button]
    private void Lose()
    {
        roomFailed?.Invoke();
    }

    private void Start()
    {
        grid = GetComponent<Grid>();
        root = new GameObject();
    }

    public void Initialize(RoomConfiguration room)
    {
        DestroyAll();
        SetUpRoom(room);
        cardManager.Initialize(room.cards);
    }

    private void DestroyAll()
    {
        Destroy(root);
        root = new GameObject();
    }

    private float multiplier;

    private void SetUpRoom(RoomConfiguration room)
    {
        var x = room.entities.Length;
        var y = room.entities[0].Length;

        multiplier = 5f / Mathf.Max(x, y);
        grid.cellSize = Vector3.one * multiplier;

        matrix = new Entity[x, y];
        if (x % 2 != 0) transform.position = new Vector3(-0.5f * multiplier, transform.position.y, 0);

        if (y % 2 != 0) transform.position = new Vector3(transform.position.x, -0.5f * multiplier, 0);

        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                DrawCell(i, j, x, y);
                GameObject g = null;
                switch (room.entities[i][j])
                {
                    case RoomConfiguration.TileType.Empty:
                        continue;
                    case RoomConfiguration.TileType.Enemy:
                        g = Instantiate(enemyPrefab);
                        break;
                    case RoomConfiguration.TileType.Wall:
                        g = Instantiate(wallPrefab);
                        break;
                    case RoomConfiguration.TileType.Player:
                        g = Instantiate(playerPrefab);
                        break;
                    case RoomConfiguration.TileType.Portal:
                        winCoordinates = new Vector2Int(i, j);
                        g = Instantiate(portalPrefab);
                        break;
                    case RoomConfiguration.TileType.Ogre:
                        g = Instantiate(ogrePrefab);
                        break;
                    case RoomConfiguration.TileType.Fire:
                        g = Instantiate(firePrefab);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var location = grid.GetCellCenterWorld(new Vector3Int(i - x / 2, (j - y / 2) * -1, 0));
                g.transform.position = location;
                g.transform.localScale *= multiplier;
                g.transform.SetParent(root.transform);
                if (room.entities[i][j] == RoomConfiguration.TileType.Portal)
                {
                    continue;
                }

                matrix[i, j] = g.GetComponent<Entity>();
            }
        }
    }

    private void DrawCell(int i, int j, int x, int y)
    {
        GameObject cell = null;
        if ((i + j) % 2 == 0)
        {
            cell = Instantiate(cellLitPrefab);
        }
        else
        {
            cell = Instantiate(cellUnlitPrefab);
        }

        cell.transform.position = grid.GetCellCenterWorld(new Vector3Int(i - x / 2, (j - y / 2) * -1, 0)) - new Vector3(0, 0, 10);
        cell.transform.localScale *= multiplier;
        cell.transform.SetParent(root.transform);
    }

    // Update is called once per frame
    private void Update()
    { }

    public void ExecuteCard(Card.CardType type)
    {
        Vector2Int direction;
        switch (type)
        {
            case Card.CardType.Up:
                direction = Vector2Int.up;
                break;
            case Card.CardType.Down:
                direction = Vector2Int.down;
                break;
            case Card.CardType.Left:
                direction = Vector2Int.left;
                break;
            case Card.CardType.Right:
                direction = Vector2Int.right;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        MoveEverything(direction);
    }

    private Entity[,] appMatrix;

    private void MoveEverything(Vector2Int direction)
    {
        var x = matrix.GetLength(0);
        var y = matrix.GetLength(1);

        appMatrix = new Entity[x, y];

        if (direction == Vector2Int.up)
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
                {
                    Step(i, j, direction);
                }
            }
        else if (direction == Vector2Int.down)
            for (var i = 0; i < x; i++)
            {
                for (var j = y - 1; j >= 0; j--)
                {
                    Step(i, j, direction);
                }
            }
        else if (direction == Vector2Int.left)
            for (var j = 0; j < y; j++)
            {
                for (var i = 0; i < x; i++)
                {
                    Step(i, j, direction);
                }
            }
        else if (direction == Vector2Int.right)
            for (var j = 0; j < y; j++)
            {
                for (var i = x - 1; i >= 0; i--)
                {
                    Step(i, j, direction);
                }
            }


        matrix = appMatrix;
    }

    private void Step(int i, int j, Vector2Int direction)
    {
        var x = matrix.GetLength(0);
        var y = matrix.GetLength(1);

        var row = i + direction.x;
        var col = j - direction.y;

        row = Mathf.Clamp(row, 0, x - 1);
        col = Mathf.Clamp(col, 0, y - 1);

        Entity entityTo = appMatrix[row, col];

        Entity entityFrom = matrix[i, j];
        if (entityFrom == null)
            return;
        switch (entityFrom.type)
        {
            case RoomConfiguration.TileType.Portal:
            case RoomConfiguration.TileType.Wall:
            case RoomConfiguration.TileType.Empty:
            case RoomConfiguration.TileType.Fire:
                appMatrix[i, j] = entityFrom;
                return;

            case RoomConfiguration.TileType.Enemy:
                if (entityTo == null)
                {
                    break;
                }

                if (entityTo.type == RoomConfiguration.TileType.Player)
                {
                    roomFailed?.Invoke();
                }
                else
                {
                    row = i;
                    col = j;
                }

                break;


            case RoomConfiguration.TileType.Player:
                if (winCoordinates == new Vector2Int(row, col))
                {
                    roomCompleted?.Invoke();
                }
                if (entityTo == null)
                    break;
                switch (entityTo.type)
                {
                    case RoomConfiguration.TileType.Ogre:
                    case RoomConfiguration.TileType.Enemy:
                    case RoomConfiguration.TileType.Fire:
                        roomFailed?.Invoke();
                        break;
                    default:
                        row = i;
                        col = j; 
                        break;
                }

               

                break;

            case RoomConfiguration.TileType.Ogre:
                if (entityTo == null)
                    break;

                if (entityTo.type == RoomConfiguration.TileType.Fire)
                {
                    //DAMAGE
                    entityFrom.hp--;
                    if (entityFrom.hp <= 0)
                    {
                        roomCompleted?.Invoke();
                    }

                    Destroy(entityTo.gameObject);
                    appMatrix[row, col] = null;
                }

                break;


                if (entityTo != null)
                {
                    switch (entityFrom.type)
                    {
                        case RoomConfiguration.TileType.Fire:
                            if (entityTo.type == RoomConfiguration.TileType.Fire)
                            {
                                //DAMAGE
                            }

                            break;

                        case RoomConfiguration.TileType.Enemy:
                            if (entityTo.type == RoomConfiguration.TileType.Player)
                            {
                                roomFailed?.Invoke();
                            }

                            break;
                        case RoomConfiguration.TileType.Player:
                            switch (entityTo.type)
                            {
                                case RoomConfiguration.TileType.Ogre:
                                case RoomConfiguration.TileType.Enemy:
                                case RoomConfiguration.TileType.Fire:
                                    roomFailed?.Invoke();
                                    break;
                            }


                            break;
                    }

                    row = i;
                    col = j;
                }

                if (entityFrom.type == RoomConfiguration.TileType.Player)
                {
                    if (winCoordinates == new Vector2Int(row, col))
                    {
                        roomCompleted?.Invoke();
                    }
                }

                appMatrix[row, col] = entityFrom;

                if (row == i && col == j)
                    return;

                // todo do movement on entity to manage animations
                entityFrom.MoveTo(grid.GetCellCenterWorld(new Vector3Int(row - x / 2, (col - y / 2) * -1, 0)), direction);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        appMatrix[row, col] = entityFrom;

        if (row == i && col == j)
            return;

        // todo do movement on entity to manage animations
        entityFrom.MoveTo(grid.GetCellCenterWorld(new Vector3Int(row - x / 2, (col - y / 2) * -1, 0)), direction);
    }
}