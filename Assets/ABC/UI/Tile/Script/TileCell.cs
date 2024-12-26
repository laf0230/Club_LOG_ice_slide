using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public Tile tile { get; set; }

    //Ÿ���� ���� ��� true
    public bool empty => tile == null;
    //�� ���� Ÿ���� ���� ��� true�� ��ȯ
    public bool occupied => tile != null; 
}
