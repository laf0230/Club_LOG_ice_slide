using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public LevelManager[] levels;
    public LevelManager activatedLevel { get; private set; }
    public Transform levelContainer;
    public int selectedLevel { get; set; } = 0;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SoundManager.Instance.PlayMusic("Normal");
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

    public void RestartLevel()
    {
        CloseLevel();
        OpenLevel(selectedLevel);
    }

    public void BackToLevelSelect()
    {
        CloseLevel();
    }

    public void OpenLevel(int levelIndex)
    {
        // levelIndex가 1 이상이고 levels 배열의 길이보다 작은지 확인
        if (levelIndex < 1 || levelIndex > levels.Length)
        {
            Debug.LogError("잘못된 레벨 인덱스입니다.");
            return;
        }

        // UI 비활성화
        UIManager.instance.GetUI(typeof(LevelUI)).gameObject.SetActive(false);

        // 해당 레벨 활성화
        var levelObject = Instantiate(levels[levelIndex - 1], levelContainer);
        levelObject.gameObject.SetActive(true);
        levelObject.OnStartLevel();
        activatedLevel = levelObject;

        // GameManager
        selectedLevel = levelIndex;
    }

    public void OpenLevel(LevelManager levelManager)
    {
        int index = System.Array.IndexOf(levels, levelManager);
        if (index >= 0)
        {
            OpenLevel(index);
        }
        else
        {
            Debug.LogWarning("해당 LevelManager가 levels 배열에 존재하지 않습니다.");
        }
    }

    public void CloseLevel()
    {
        Destroy(activatedLevel.gameObject);
    }
}

