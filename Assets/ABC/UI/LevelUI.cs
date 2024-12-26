using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : UI
{
    [SerializeField] private LevelElementUI levelElementPrefab;
    [SerializeField] private Transform levelElementContainer;
    public List<LevelElementUI> levelElements = new List<LevelElementUI>();

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < GameManager.instance.gameCount; i++)
        {
            levelElements.Add(Instantiate(levelElementPrefab, levelElementContainer));
            levelElements[i].SetNumber(i);
        }
    }

    public override void AddOpenButton(Button button)
    {
        base.AddExitButton(button);
    }
}
