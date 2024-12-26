using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : UI
{
    [SerializeField] private Button gameEndButton;

    public override void Start()
    {
        base.Start();

        // 게임 종료 코드
        gameEndButton.onClick.RemoveAllListeners();
        gameEndButton.onClick.AddListener(OnGameEndButtonClick);
    }

    public void OnGameEndButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
