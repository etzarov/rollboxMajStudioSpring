using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStyleManager : MonoBehaviour
{
    public static PlayerStyleManager main;

    public SpriteRenderer outlineSR;
    public SpriteRenderer centralSR;
    public SpriteRenderer pupilSR;
    public SpriteRenderer blinkSR;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        if (SettingsManager.main != null)
        {
            UpdateStyle(SettingsManager.main.playerStyle);
        }
    }

    public void UpdateStyle(PlayerStyle style)
    {
        centralSR.sprite = style.centralSprite;
        centralSR.color = style.centralColor;
        blinkSR.color = style.centralColor;

        outlineSR.color = style.outlineColor;
        pupilSR.color = style.pupilColor;
        blinkSR.enabled = false;
    }
}
