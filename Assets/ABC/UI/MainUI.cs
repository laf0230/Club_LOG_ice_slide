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

        // ���� ���� �ڵ�
        gameEndButton.onClick.RemoveAllListeners();
        gameEndButton.onClick.AddListener(OnGameEndButtonClick);
    }

    public void OnGameEndButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
}
