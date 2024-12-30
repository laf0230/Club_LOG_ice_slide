using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearUI : UI
{
    public Button restartButton;
    public Button listButton;

    public override void Start()
    {
        base.Start();

        UIManager.instance.GetUI(typeof(LevelUI)).AddOpenButton(listButton);
        restartButton.onClick.AddListener(OnRestartButtonClick);
        listButton.onClick.AddListener(OnListButtonClick);
    }

    public void OnRestartButtonClick()
    {
        GameManager.instance.RestartLevel();
    }

    public void OnListButtonClick()
    {
        GameManager.instance.CloseLevel();
        UIManager.instance.GetUI(typeof(OnPlayUI)).OnExitButtonClick();
    }
}
