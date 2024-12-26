using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UI
{
    public Button gameEndButton;

    public override void Start()
    {
        base.Start();

        gameEndButton.onClick.RemoveAllListeners();
        gameEndButton.onClick.AddListener(OnGameEndButtonClick);
    }

    private void OnGameEndButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
