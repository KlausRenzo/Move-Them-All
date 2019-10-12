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
    [SerializeField] private List<RoomConfiguration> rooms = new List<RoomConfiguration>();

    private Grid grid;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private CardsManager cardManager;

    [TableMatrix] public Entity[,] matrix;

    [Button(Name = "Get Rooms")]
    private void GetAllRooms()
    {
        rooms = Utilities.GetAllInstances<RoomConfiguration>().ToList();
    }

    private void Start()
    {
        grid = GetComponent<Grid>();
        SetUpRoom();
        cardManager.Initialize(rooms[0].cards);
    }

    private void SetUpRoom()
    {
        var roomConfiguration = rooms[0];
        var x = roomConfiguration.entities.GetLength(0);
        var y = roomConfiguration.entities.GetLength(1);
        matrix = new Entity[x, y];
        if (x % 2 != 0) transform.position = new Vector3(-0.5f, transform.position.y, 0);

        if (y % 2 != 0) transform.position = new Vector3(transform.position.x, -0.5f, 0);

        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                GameObject g = null;
                switch (roomConfiguration.entities[i, j])
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var location = grid.GetCellCenterWorld(new Vector3Int(i - x / 2, (j - y / 2) * -1, 0));
                g.transform.position = location;
                g.transform.SetParent(transform);
                matrix[i, j] = g.GetComponent<Entity>();
            }
        }
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
            for (var i = x - 1; i >= 0; i--)
            {
                for (var j = 0; j < y; j++)
                {
                    Step(i, j, direction);
                }
            }
        else if (direction == Vector2Int.left)
            for (var i = 0; i < x; i++)
            {
                for (var j = y - 1; j >= 0; j--)
                {
                    Step(i, j, direction);
                }
            }
        else if (direction == Vector2Int.right)
            for (var i = 0; i < x; i++)
            {
                for (var j = 0; j < y; j++)
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
                break;
            case RoomConfiguration.TileType.Enemy:
            case RoomConfiguration.TileType.Player:
            case RoomConfiguration.TileType.Ogre:
                var row = i + direction.x;
                var col = j - direction.y;

                row = Mathf.Clamp(row, 0, x - 1);
                col = Mathf.Clamp(col, 0, y - 1);

                Entity entityTo = appMatrix[row, col];
                if (entityTo != null)
                {
                    switch (entityFrom.type)
                    {
                        case RoomConfiguration.TileType.Ogre:
                            if (entityTo.type == RoomConfiguration.TileType.Fire)
                            {
                                //DAMAGE
                            }

                            break;

                        case RoomConfiguration.TileType.Enemy:
                            if (entityTo.type == RoomConfiguration.TileType.Player)
                            {
                                //DEAD
                            }

                            break;
                        case RoomConfiguration.TileType.Player:
                            switch (entityTo.type)
                            {
                                case RoomConfiguration.TileType.Ogre:
                                case RoomConfiguration.TileType.Enemy:
                                case RoomConfiguration.TileType.Fire:
                                    //DEAD
                                    break;
                                case RoomConfiguration.TileType.Portal:
                                    //WIN
                                    break;
                            }

                            break;
                    }

                    row = i;
                    col = j;
                }

                appMatrix[row, col] = entityFrom;

                if (row == i && col == j)
                    return;
                // todo do movement on entity to manage animations
                entityFrom.transform.position = grid.GetCellCenterWorld(new Vector3Int(row - x / 2, (col - y / 2) * -1, 0));

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}