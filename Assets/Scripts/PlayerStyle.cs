using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "P_Style", menuName = "Player Style")]
public class PlayerStyle : ScriptableObject
{
    public Color centralColor;
    public Color outlineColor;
    public Color pupilColor;
    [Space]
    public Sprite centralSprite;
}
