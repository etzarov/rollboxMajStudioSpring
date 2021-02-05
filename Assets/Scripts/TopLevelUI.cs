using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopLevelUI : MonoBehaviour
{
    public static TopLevelUI main;

    private void Awake()
    {
        main = this;
    }


    public TextMeshPro totalCrateCount;
    public TextMeshPro starTargetTwo;
    public TextMeshPro starTargetThree;

    [Space]
    public SpriteRenderer[] starSRs;
    public SpriteRenderer[] starOutlineSRs;


    private void Start()
    {
        UpdateStarTargetCounts();
        UpdateStarDisplay();
        UpdateCrateCount();
    }


    void UpdateStarTargetCounts()
    {
        LevelSelectInfo lSelectInfo = CurrentLevelManager.main.thisLevel;
        starTargetTwo.text = lSelectInfo.maxCratesFor2Stars.ToString();
        starTargetThree.text = lSelectInfo.maxCratesFor3Stars.ToString();
    }

    public void UpdateStarDisplay()
    {
        int totalStars = GetStarCount();
        Palette p = CurrentLevelManager.main.generalPalette;

        for (int i = 0; i < starSRs.Length; i++)
        {
            if (i < totalStars)
            {
                starSRs[i].color = p.starUnlocked;
            }
            else
            {
                starSRs[i].color = p.starLocked;
            }
            starOutlineSRs[i].color = p.buttonOutline;
        }
    }

    public int GetStarCount()
    {
        int usedCrates = CurrentLevelManager.main.totalCratesDropped;
        LevelSelectInfo lSelectInfo = CurrentLevelManager.main.thisLevel;


        int totalStars = 1;

            if (usedCrates <= lSelectInfo.maxCratesFor2Stars)
            {
                totalStars = 2;
            }
            if (usedCrates <= lSelectInfo.maxCratesFor3Stars)
            {
                totalStars = 3;
            }



        return totalStars;
    }

    public void UpdateCrateCount()
    {
        totalCrateCount.text = CurrentLevelManager.main.totalCratesDropped.ToString();
        UpdateStarDisplay();
    }

}
