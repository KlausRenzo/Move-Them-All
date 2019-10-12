﻿using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Internet Festival/Room")]
public class RoomConfiguration : SerializedScriptableObject
{
    public int roomNumber;

    [TableMatrix(HorizontalTitle = "X axis")]
    public TileType[,] EnumTable = new TileType[4, 4];
    
    public Card[] cards;

    public enum TileType
    {
        Empty,
        Enemy,
        Wall,
        Player
    }
}

[System.Serializable]
public class RoomTile
{
    public int x;
    public int y;
}

[System.Serializable]
public class Card
{
    public enum CardType
    {
        Up,
        Down,
        Left,
        Right
    };

    public CardType type;
}