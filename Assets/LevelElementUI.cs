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
        // levelNumber�� 1 �̻��� ���� OpenLevel ȣ��
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
                Debug.LogError("���� ��ȣ�� ��ȿ���� �ʽ��ϴ�.");
            }
        });
    }

    public void SetNumber(int number)
    {
        levelNumber = number;
        textMeshPro.text = number.ToString(); // Update the level number text
    }
}
