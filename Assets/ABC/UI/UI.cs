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
        this.gameObject.SetActive(false);
    }

    public virtual void OnOpenButtonClick()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void AddOpenButton(Button button)
    {
        openButton.Add(button);
        InitializeButtonOpenExit();
    }

    public virtual void AddExitButton(Button button)
    {
        exitButton.Add(button);
        InitializeButtonOpenExit();
    }

    #endregion
}
