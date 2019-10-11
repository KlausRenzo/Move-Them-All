using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InternetFestival/Room")]
public class RoomConfiguration : ScriptableObject
{
    public int roomX;
    public int roomY;

    public RoomTile[,] matrix;
    
    public Card[] cards;

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