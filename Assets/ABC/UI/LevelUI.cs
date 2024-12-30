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

        // Ensure the levelElementPrefab and levelElementContainer are assigned
        if (levelElementPrefab == null)
        {
            Debug.LogError("Level Element Prefab is not assigned.");
            return;
        }
        if (levelElementContainer == null)
        {
            Debug.LogError("Level Element Container is not assigned.");
            return;
        }

        for (int i = 0; i < GameManager.instance.levels.Length; i++)
        {
            // Instantiate the level element and add it to the list
            LevelElementUI newLevelElement = Instantiate(levelElementPrefab, levelElementContainer);
            newLevelElement.SetNumber(i + 1); // Set the number (starting from 1)
            levelElements.Add(newLevelElement);
        }
    }
}
