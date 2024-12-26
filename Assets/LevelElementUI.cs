using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelElementUI : MonoBehaviour
{
    private Button button;
    public TextMeshProUGUI textMeshPro { get; private set; }

    private void OnEnable()
    {
        button = GetComponent<Button>();
        textMeshPro = transform.GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(OpenGamePlay);
    }

    public void OpenGamePlay()
    {
        UIManager.instance.GetUI(typeof(GameEntityUI)).AddOpenButton(button);
    }

    public void SetNumber(int number)
    {
        textMeshPro.text = number.ToString();
    }
}
