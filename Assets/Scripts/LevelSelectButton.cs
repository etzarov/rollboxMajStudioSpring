using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    public LevelSelectInfo levelToTravelTo;
    [HideInInspector] public bool unlocked;
    [Space(25)]
    public BoxCollider touchHitbox;
    [Space]
    public SpriteRenderer outsideButtonSR;
    public SpriteRenderer insideButtonSR;
    [Space]
    public Transform starHolder;
    public SpriteRenderer[] starSRs;
    public SpriteRenderer[] starOutlineSRs;
    [Space]
    public TextMeshPro levelNumberText;
    public SpriteRenderer lockIcon;

    /// <summary>
    /// Updates the current colors and star display of the level button.
    /// </summary>
    /// <param name="usedPalette">The color palette to use for the button.</param>
    public void UpdateDisplay(Palette usedPalette, int levelNumber)
    {
        Palette p = usedPalette;

        if (unlocked)
        {
            starHolder.gameObject.SetActive(true);
            UpdateStars(p);

            outsideButtonSR.color = p.buttonOutline;
            insideButtonSR.color = p.buttonInside;

           
            levelNumberText.text = levelNumber.ToString();
            levelNumberText.color = p.text;
            lockIcon.enabled = false;
        }
        else
        {
            starHolder.gameObject.SetActive(false);

            outsideButtonSR.color = p.buttonLockedOutline;
            insideButtonSR.color = p.buttonLockedInside;
            lockIcon.enabled = true;
            lockIcon.color = p.text;
            levelNumberText.text = "";
        }
    }

    /// <summary>
    /// Updates the displayed star score
    /// </summary>
    /// <param name="p">The palette to use for stars</param>
    public void UpdateStars(Palette p)
    {
        int totalStars = GetStarCount();
        for (int i = 0; i < 3; i++)
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
        int usedCrates = levelToTravelTo.levelCompletionData.cratesUsed;
        int totalStars = 0;
        if (levelToTravelTo.levelCompletionData.completed)
        {
            totalStars = 1;

            if (usedCrates <= levelToTravelTo.maxCratesFor2Stars)
            {
                totalStars = 2;
            }
            if (usedCrates <= levelToTravelTo.maxCratesFor3Stars)
            {
                totalStars = 3;
            }
        }

        levelToTravelTo.levelCompletionData.earnedStars = totalStars;

        return totalStars;
    }
}
