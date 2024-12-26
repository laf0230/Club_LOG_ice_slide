using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBlock : MonoBehaviour
{
    public TileState state;
    public TileCell cell { get; set; }

    private Image background;
    private Text text;

    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponent<Text>();
    }

    public void SetState(TileState state)
    {
        this.state = state;
    }
}
