using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static LevelSelectManager main;

    [Header("Level Select")]
    public LevelSelectButton[] allLevels;
    public BonusLevelConnector[] bonusLevels;
    [Header("Level Select Palettes")]
    public Palette grassyPalette;
    [Space]
    public LineRenderer lineRenderer;

    // Start is called before the first frame update

    private void Awake()
    {
        main = this;
    }
    void Start()
    {
        GenerateLineConnectors();
        UpdateLevelDisplays();
        LoadData(true);
    }

    // Update is called once per frame
    void Update()
    {
        DetectClicks();
        UpdateLevelDisplays();

        if (Input.GetKeyDown(KeyCode.P))
        {
            UnlockAll();
        }
    }

    void UnlockAll()
    {
        foreach (var level in allLevels)
        {
            level.unlocked = true;
            level.levelToTravelTo.levelCompletionData.completed = true;
        }

        foreach (var level in bonusLevels)
        {
            level.levelSelectButton.unlocked = true;
            level.levelSelectButton.levelToTravelTo.levelCompletionData.completed = true;
        }

        SaveAllData();
        UpdateLevelDisplays();
    }

    void UpdateLevelDisplays()
    {
        UpdateUnlocks();
        

        int levelNo = 1;
        for (int i = 0; i < allLevels.Length; i++)
        {
            allLevels[i].UpdateDisplay(grassyPalette,levelNo.ToString());
            levelNo++;
        }
        foreach (var bonusLevel in bonusLevels)
        {
            bonusLevel.levelSelectButton.UpdateDisplay(grassyPalette, (bonusLevel.connectedLevelIndex + 1).ToString() + "a");
        }
    }


    /// <summary>
    /// Updates which levels are able to be unlocked and which are not. Currently, a level unlocks if the previous one is beaten.
    /// </summary>
    void UpdateUnlocks()
    {
        bool previousCompleted = true;
        for (int i = 0; i < allLevels.Length; i++)
        {
            allLevels[i].unlocked = previousCompleted;

            previousCompleted = allLevels[i].levelToTravelTo.levelCompletionData.completed;
        }

        foreach (var bonusLevel in bonusLevels)
        {
            if (allLevels[bonusLevel.connectedLevelIndex].unlocked)
            {
                bonusLevel.levelSelectButton.unlocked = true;
            }
        }
    }

    void DetectClicks()
    {
        bool pressed = false;
        Vector3 touchPos = Vector3.zero;
        ExtensionMethods.DetectTouches(out pressed, out touchPos);

        if (pressed)
        {
            for (int i = 0; i < allLevels.Length; i++)
            {
                if (ExtensionMethods.TouchedHitbox(allLevels[i].touchHitbox, touchPos))
                {
                    if (allLevels[i].unlocked)
                    {
                        //TransitionManager.main.Transition();
                        SceneManager.LoadScene(allLevels[i].levelToTravelTo.buildSceneNumber);
                        break;
                    }
                }
            }

            foreach (var bonusLevel in bonusLevels)
            {
                if (ExtensionMethods.TouchedHitbox(bonusLevel.levelSelectButton.touchHitbox, touchPos))
                {
                    if (bonusLevel.levelSelectButton.unlocked)
                    {
                        //TransitionManager.main.Transition();
                        SceneManager.LoadScene(bonusLevel.levelSelectButton.levelToTravelTo.buildSceneNumber);
                        break;
                    }
                }
            }
        }
    }


    public void LoadData(bool shouldSave)
    {
        foreach (LevelSelectButton levelSelect in allLevels)
        {
            LevelSelectInfo lSelect = levelSelect.levelToTravelTo;
            string loadString = "LevelData_" + lSelect.buildSceneNumber.ToString();
            if (ES3.KeyExists(loadString))
            {
                LevelSaveData loadedData = ES3.Load<LevelSaveData>(loadString);
                lSelect.levelCompletionData = new LevelCompletionData();
                lSelect.levelCompletionData.completed = loadedData.completed;
                lSelect.levelCompletionData.earnedStars = loadedData.stars;
                lSelect.levelCompletionData.cratesUsed = loadedData.cratesDropped;
            }
            else
            {
                lSelect.levelCompletionData = new LevelCompletionData();
                lSelect.levelCompletionData.completed = false;
                lSelect.levelCompletionData.earnedStars = 0;
                lSelect.levelCompletionData.cratesUsed = 0;
                if (shouldSave)
                {
                    SaveProgress(false, 0, 0, true, lSelect);
                }
            }

            levelSelect.UpdateStars(grassyPalette);
        }
        UpdateUnlocks();
    }

    public void SaveProgress(bool completed, int droppedCrates, int stars, bool shouldLoad, LevelSelectInfo lSave)
    {
        LoadData(false);
        string saveString = "LevelData_" + lSave.buildSceneNumber.ToString();
        LevelSaveData lSaveData = new LevelSaveData();
        lSaveData.stars = stars;
        lSaveData.cratesDropped = droppedCrates;
        lSaveData.completed = completed;

        ES3.Save<LevelSaveData>(saveString, lSaveData);
    }

    public void SaveAllData()
    {
        foreach (var level in allLevels)
        {
            SaveProgress(level.unlocked, level.levelToTravelTo.levelCompletionData.cratesUsed, level.levelToTravelTo.levelCompletionData.earnedStars, false, level.levelToTravelTo);
        }

        foreach (var bonusLevel in bonusLevels)
        {
            LevelSelectButton level = bonusLevel.levelSelectButton;
            SaveProgress(level.unlocked, level.levelToTravelTo.levelCompletionData.cratesUsed, level.levelToTravelTo.levelCompletionData.earnedStars, false, level.levelToTravelTo);
        }
    }

    void GenerateLineConnectors()
    {
        int totalLevels = allLevels.Length;
        lineRenderer.positionCount = totalLevels;
        for (int i = 0; i < allLevels.Length; i++)
        {
            Vector3 tPos = allLevels[i].transform.position;
            tPos.z = .1f;
            lineRenderer.SetPosition(i, tPos);
        }
        foreach (var bonusLevel in bonusLevels)
        {
            bonusLevel.connectedLine.positionCount = 2;
            Vector3 tPos = bonusLevel.levelSelectButton.transform.position;
            tPos.z = .1f;
            bonusLevel.connectedLine.SetPosition(0, tPos);
            tPos = allLevels[bonusLevel.connectedLevelIndex].transform.position;
            tPos.z = .1f;
            bonusLevel.connectedLine.SetPosition(1, tPos);
        }
    }

}


[System.Serializable]
public class BonusLevelConnector
{
    public LevelSelectButton levelSelectButton;
    [Tooltip("The current level that it branches off from")]
    public int connectedLevelIndex;

    public LineRenderer connectedLine;
}

//[System.Serializable]
//public class ProgressionBarricade
//{
//    int levelNumberBarricaded;
//    int totalStarsRequiredToProgress;
//}