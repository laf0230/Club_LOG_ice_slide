using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public int gameCount = 0;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        board.ClearBoard();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
    }
}
