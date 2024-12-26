using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public Tile tile { get; set; }

    //타일이 없을 경우 true
    public bool empty => tile == null;
    //셀 위에 타일이 있을 경우 true를 반환
    public bool occupied => tile != null; 
}
