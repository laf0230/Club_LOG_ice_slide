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
        // levelIndex�� 1 �̻��̰� levels �迭�� ���̺��� ������ Ȯ��
        if (levelIndex < 1 || levelIndex > levels.Length)
        {
            Debug.LogError("�߸��� ���� �ε����Դϴ�.");
            return;
        }

        // UI ��Ȱ��ȭ
        UIManager.instance.GetUI(typeof(LevelUI)).gameObject.SetActive(false);

        // �ش� ���� Ȱ��ȭ
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
            Debug.LogWarning("�ش� LevelManager�� levels �迭�� �������� �ʽ��ϴ�.");
        }
    }

    public void CloseLevel()
    {
        Destroy(activatedLevel.gameObject);
    }
}

