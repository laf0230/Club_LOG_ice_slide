using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile State", menuName = "New Tile State")]
public class TileState : ScriptableObject
{
    public int number;
    public Color backgroundColor;
    public Color textColor;
}
