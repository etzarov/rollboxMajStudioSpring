﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentLevelManager : MonoBehaviour
{

    public static CurrentLevelManager main;
    public LevelSelectInfo thisLevel;
    public Palette generalPalette;
    public int totalCratesDropped;
    [Space]
    public bool mustReset;
    public SpriteRenderer resetDisplay;

    public bool levelComplete;
    public LevelCompleteDisplay levelCompleteDisplay;

    public TNTScript tnt;
    public ButtonScript button;
    public BoxCollider resetButton;
    public BoxCollider homeButton;
    public BoxCollider nextLevelButton;



    private void Awake()
    {
        main = this;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || ExtensionMethods.TouchedHitbox(resetButton))
        {
            ResetGame();
        }

        if (Input.GetKeyDown(KeyCode.H) || ExtensionMethods.TouchedHitbox(homeButton))
        {
            if (TransitionManager.main)
            {
                TransitionManager.main.Transition();
            }
            SceneManager.LoadScene(0);

        }

        if (Input.GetKeyDown(KeyCode.P) || ExtensionMethods.TouchedHitbox(homeButton))
        {
            LevelCompleted();

        }



        resetDisplay.enabled = mustReset;
        //levelCompleteDisplay.gameObject.SetActive(levelComplete);
    }

    public void GoToMenu()
    {
        if (TransitionManager.main)
        {
            TransitionManager.main.Transition();
        }
        SceneManager.LoadScene(0);
    }

    public void GoToNextLevel()
    {
        if (TransitionManager.main != null)
        {
            TransitionManager.main.Transition();
        }
        SceneManager.LoadScene(thisLevel.nextLevel.buildSceneNumber);
    }

    public void ResetGame()
    {
        //Added by Taiyo Baniecki Reset the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (CraneManagement.main != null)
        {
            CraneManagement.main.ClearAllCrates();
        }

        if (CrateSpawner.main != null) {
            CrateSpawner.main.ResetCrates();
        }

        if (PlayerManager.main != null)
        {
            PlayerManager.main.ResetPlayer();
        }

        if (tnt != null)
        {
            tnt.gameObject.SetActive(true);
        }

        if (button != null)
        {
            button.buttonPressed = false;
        }

        totalCratesDropped = 0;
        TopLevelUI.main.UpdateCrateCount();

        mustReset = false;
        levelComplete = false;
        levelCompleteDisplay.gameObject.SetActive(false);
    }

    public void AddCrate()
    {
        if (!levelComplete)
        {
            totalCratesDropped++;
            TopLevelUI.main.UpdateCrateCount();
        }
    }

    public void LevelCompleted()
    {
        int totalStars = 0;
        if (!levelComplete)
        {
            if (FireworkManager.main!=null)
            {
                FireworkManager.main.LaunchParticles();
            }

            

            levelComplete = true;
            thisLevel.levelCompletionData.completed = true;
             totalStars = GetStars(thisLevel.levelCompletionData.cratesUsed);
            int droppedCrates = thisLevel.levelCompletionData.cratesUsed;
            if (totalCratesDropped < thisLevel.levelCompletionData.cratesUsed)
            {
                totalStars = GetStars(totalCratesDropped);
                thisLevel.levelCompletionData.cratesUsed = totalCratesDropped;
                droppedCrates = totalCratesDropped;
            }
            else if (thisLevel.levelCompletionData.cratesUsed == 0)
            {
                totalStars = GetStars(totalCratesDropped);
                thisLevel.levelCompletionData.cratesUsed = totalCratesDropped;
                droppedCrates = totalCratesDropped;
            }

            

            SaveProgress(true, droppedCrates, totalStars,true);
        }
        levelCompleteDisplay.gameObject.SetActive(true);
        levelCompleteDisplay.SetLevelComplete(GetStars(totalCratesDropped));
    }

    int GetStars(int cratesDropped)
    {
        if (cratesDropped <= thisLevel.maxCratesFor3Stars)
        {
            return 3;
        }
        else if (cratesDropped <= thisLevel.maxCratesFor2Stars)
        {
            return 2;
        }
        else return 1;

    }

    public void SaveProgress(bool completed, int droppedCrates, int stars, bool shouldLoad)
    {
        
        if (shouldLoad) {
            LoadData();
        }
        string saveString = "LevelData_" + thisLevel.buildSceneNumber.ToString();
        print("SAVE " + saveString + " - Dropped Crates: " + droppedCrates);
        LevelSaveData lSaveData = new LevelSaveData();
        lSaveData.stars = stars;
        lSaveData.cratesDropped = droppedCrates;
        lSaveData.completed = completed;

        print("saved");
        ES3.Save<LevelSaveData>(saveString, lSaveData);
    }

    public void LoadData()
    {
        string loadString = "LevelData_" + thisLevel.buildSceneNumber.ToString();
        print("Load: " + loadString);
        if (ES3.KeyExists(loadString))
        {
            LevelSaveData loadedData = ES3.Load<LevelSaveData>(loadString);
            thisLevel.levelCompletionData = new LevelCompletionData();
            thisLevel.levelCompletionData.completed = loadedData.completed;
            thisLevel.levelCompletionData.earnedStars = loadedData.stars;
            thisLevel.levelCompletionData.cratesUsed = loadedData.cratesDropped;
            print("Key found: loaded " + loadedData.cratesDropped + " crates dropped");

        }
        else
        {
            thisLevel.levelCompletionData = new LevelCompletionData();
            thisLevel.levelCompletionData.completed = false;
            thisLevel.levelCompletionData.earnedStars = 0;
            thisLevel.levelCompletionData.cratesUsed = 0;

            SaveProgress(false,0,0,false);
        }
    }

    public bool Completed()
    {
        return levelComplete;
    }
}

[System.Serializable]
public class LevelSaveData
{
    public int stars;
    public bool completed;
    public int cratesDropped;
}
