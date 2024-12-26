using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Tile/TileData")]
public class TileData : ScriptableObject
{
    public float speed;
    public LayerMask targetLayer;
}
