using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public UIType uiType;
    public List<Button> exitButton;
    public List<Button> openButton;

    public virtual void Start()
    {
        InitializeButtonOpenExit();
    }

    #region Open/Exit Button

    public void InitializeButtonOpenExit()
    {
        // 버튼 기능 초기화
        foreach (var button in exitButton)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnExitButtonClick);
        }

        foreach (var button in openButton)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnOpenButtonClick);
        }
    }

    public virtual void OnExitButtonClick()
    {
        // 닫기 버튼 클릭 이벤트
        this.gameObject.SetActive(false);
    }

    public virtual void OnOpenButtonClick()
    {
        // 열기 버튼 클릭 이벤트
        this.gameObject.SetActive(true);
    }

    public virtual void AddOpenButton(Button button)
    {
        // 여는 버튼 추가
        openButton.Add(button);
    }

    public virtual void AddExitButton(Button button)
    {
        // 닫는 버튼 추가
        exitButton.Add(button);
    }

    #endregion
}
