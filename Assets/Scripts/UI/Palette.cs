using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Palette", menuName = "Palette")]
public class Palette : ScriptableObject
{
    public Color text;
    [Space]

    public Color buttonOutline;
    public Color buttonInside;
    [Space]
    public Color buttonLockedOutline;
    public Color buttonLockedInside;
    [Space]
    public Color starUnlocked;
    public Color starLocked;
    
}
