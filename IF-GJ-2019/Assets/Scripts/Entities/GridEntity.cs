using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : ScriptableObject
{
    public Sprite sprite;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Internet Festival/Grid Entities/Enemy")]
public class GridEnemy : GridEntity
{ }

[CreateAssetMenu(menuName = "Internet Festival/Grid Entities/Wall")]
public class GridWall : GridEntity
{ }
[CreateAssetMenu(menuName = "Internet Festival/Grid Entities/Player")]
public class GridPlayer : GridEntity
{ }
