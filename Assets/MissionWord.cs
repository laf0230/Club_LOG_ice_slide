using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionWord : MonoBehaviour
{
    public TextMeshProUGUI text { get; set; }
    public string word => text.text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
