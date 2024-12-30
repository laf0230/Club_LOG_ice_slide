using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UI
{
    public Button gameEndButton;

    private void OnGameEndButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void ToggleUI()
    {
        if (gameObject.activeSelf)
        {
            OnExitButtonClick();
        }
        else
        {
            OnOpenButtonClick();
        }
    }
}
