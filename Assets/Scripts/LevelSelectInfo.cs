using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Data")]
public class LevelSelectInfo : ScriptableObject
{
    public int buildSceneNumber;
    public LevelSelectInfo nextLevel;
    [Space]
    public int maxCratesFor3Stars;
    public int maxCratesFor2Stars;
    [Space]
    public LevelCompletionData levelCompletionData;
}

[System.Serializable]
public class LevelCompletionData
{
    public bool completed;
    public int cratesUsed;
    public int earnedStars;
}
