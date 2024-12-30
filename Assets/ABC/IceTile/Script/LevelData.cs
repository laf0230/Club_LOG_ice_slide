using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject m_Tile;
    public string m_MissionWord;
}
