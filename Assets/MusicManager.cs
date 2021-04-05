using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MusicManager : MonoBehaviour
{
    public static MusicManager main;

    public AudioSource aSrc;
    public float masterVolume;

    public AudioClip grassAudio;
    public AudioClip westernAudio;
    public AudioClip caveAudio;

    AudioClip targetAudio;

    int cScene;
    private void Awake()
    {
        if (main != null && main != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            main = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        cScene = SceneManager.GetActiveScene().buildIndex;
        if (cScene == 0)
        {
            targetAudio = null;
        }
        else
        {
            if (CurrentLevelManager.main)
            {
                targetAudio = GetAudioClip(CurrentLevelManager.main.thisLevel.levelZone);
            }
           
        }
        if (aSrc.clip != targetAudio)
        {
            aSrc.volume -= Time.deltaTime/6f;
            if (aSrc.volume <= .01f)
            {
                aSrc.Stop();
                aSrc.clip = targetAudio;
                if (targetAudio != null)
                {
                    aSrc.Play();
                }
            }
        }
        else
        {
            aSrc.volume = Mathf.Min(aSrc.volume + Time.deltaTime, masterVolume * .4f);
        }
    }

    public void UpdateMusicVolume()
    {
        masterVolume = SettingsManager.main.musicVolume;
        aSrc.volume = masterVolume * .4f;
    }

    AudioClip GetAudioClip(LevelSelectInfo.LevelZone lz)
    {
        switch (lz)
        {
            case LevelSelectInfo.LevelZone.None: return targetAudio;
            case LevelSelectInfo.LevelZone.Grassy: return grassAudio;
            case LevelSelectInfo.LevelZone.Western: return westernAudio;
            case LevelSelectInfo.LevelZone.Caverns: return caveAudio;
            default: return targetAudio;
        }
    }
}
