using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelElementUI : MonoBehaviour
{
    private Button button;
    public TextMeshProUGUI textMeshPro;
    public int levelNumber;

    public void Start()
    {
        button = GetComponent<Button>();
        UIManager.instance.GetUI(typeof(LevelUI)).AddExitButton(button);
        // levelNumber가 1 이상일 때만 OpenLevel 호출
        OnButtonClick();
    }

    public void OnButtonClick()
    {
        button.onClick.AddListener(() =>
        {
            if (levelNumber >= 1)
            {
                GameManager.instance.OpenLevel(levelNumber);
                UIManager.instance.GetUI(typeof(OnPlayUI)).OnOpenButtonClick();
            }
            else
            {
                Debug.LogError("레벨 번호가 유효하지 않습니다.");
            }
        });
    }

    public void SetNumber(int number)
    {
        levelNumber = number;
        textMeshPro.text = number.ToString(); // Update the level number text
    }
}
