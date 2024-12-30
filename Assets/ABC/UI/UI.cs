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
        // ��ư ��� �ʱ�ȭ
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
        // �ݱ� ��ư Ŭ�� �̺�Ʈ
        this.gameObject.SetActive(false);
    }

    public virtual void OnOpenButtonClick()
    {
        // ���� ��ư Ŭ�� �̺�Ʈ
        this.gameObject.SetActive(true);
    }

    public virtual void AddOpenButton(Button button)
    {
        // ���� ��ư �߰�
        openButton.Add(button);
    }

    public virtual void AddExitButton(Button button)
    {
        // �ݴ� ��ư �߰�
        exitButton.Add(button);
    }

    #endregion
}
