using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    main,
    level,
    setting,
    gamePlay
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private List<UI> uis;

    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var item in uis)
        {
            if((item.GetType() == typeof(MainUI)))
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
    }

    public UI GetUI(Type type)
    {
        foreach (var item in uis)
        {
            if(item.GetType() == type)
                return item;
        }

        // Null
        Debug.Log($"{type}�ش� UI Ÿ���� �������� �ʽ��ϴ�.");
        return null;
    }
}
