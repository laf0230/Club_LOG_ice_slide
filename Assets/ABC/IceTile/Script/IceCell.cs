using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IceCell : TileBase
{
    public Vector2Int coordinates;
    private IceTile tile;
    public bool occupied => tile != null;
    public bool blocked => tile != null;
}
