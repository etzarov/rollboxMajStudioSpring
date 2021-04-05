using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    [Range(0, 30)]
    public float currentHeight;
    public bool inMenu;
    [Space]
    public MenuMusicSection[] sections;
    [Space]
    public AudioSource musicSrc;
    public AudioLowPassFilter lpFilter;
    public AudioHighPassFilter hpFilter;
    [Space]
    public Transform radioSpeaker;
    public AnimationCurve radioCurve;

    void Update()
    {
        UpdateMainMusic();
        UpdateSections();
        if (inMenu)
        {
            UpdateRadioBumping();
        }
    }

    void UpdateSections()
    {

        foreach (var section in sections)
        {
            float targetVolume = 0;
            if (!inMenu)
            {
                if (currentHeight <= section.upperHeight && currentHeight >= section.lowerHeight)
                {
                    float t = Remap(currentHeight, section.lowerHeight, section.upperHeight, 0, 1);
                    section.audioSource.volume = section.volumeFalloff.Evaluate(t) * section.maxVolume;
                }
                else
                {
                    section.audioSource.volume = 0;
                }
            }
            targetVolume *= SettingsManager.main.musicVolume;
            section.audioSource.volume = Mathf.Lerp(section.audioSource.volume, targetVolume, .01f);
        }
    }

    void UpdateMainMusic()
    {
        float targetLP = inMenu ? 21000 : 4784;
        float targetHP = inMenu ? 10 : 2433;
        float targetPan = inMenu ? 0 : -.75f;
        float targetVolume = .95f * SettingsManager.main.musicVolume;

        if (!inMenu)
        {
            targetVolume *= Mathf.Clamp01(Remap(currentHeight, 0, 7, 1, 0));
        }

        musicSrc.volume = Mathf.Lerp(musicSrc.volume, targetVolume, .02f) ;
        musicSrc.panStereo = Mathf.Lerp(musicSrc.panStereo, targetPan, .005f);
        lpFilter.cutoffFrequency = Mathf.Lerp(lpFilter.cutoffFrequency, targetLP, .005f);
        hpFilter.cutoffFrequency = Mathf.Lerp(hpFilter.cutoffFrequency, targetHP, .005f);


    }

    void UpdateRadioBumping()
    {
        radioSpeaker.localScale = Vector3.one * (radioCurve.Evaluate(Time.time % 3 / 3));
    }

    public void MoveToLevelMap()
    {
        inMenu = false;
    }

    public void MoveToMenu()
    {
        inMenu = true;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}

[System.Serializable]
public class MenuMusicSection
{
    public string name;
    public AudioSource audioSource;
    public float lowerHeight;
    public float upperHeight;
    public AnimationCurve volumeFalloff;
    public float maxVolume;
}
