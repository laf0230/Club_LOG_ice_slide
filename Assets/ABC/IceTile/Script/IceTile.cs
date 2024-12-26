using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IceTile : MonoBehaviour
{
    public IceCell iceCell { get; set; }
    public bool locked { get; set; }

    private Text text;
    private Image image;
}
