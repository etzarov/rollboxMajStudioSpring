using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager main;

    private void Awake()
    {
        if (main!=null && main != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            main = this;
            DontDestroyOnLoad(this);
        }
    }

    public float soundVolume;
    public float musicVolume;
    public PlayerStyle playerStyle;


    private void Start()
    {
        soundVolume = .75f;
        musicVolume = .75f;
        LoadSettings();
    }

    public void SaveSettings()
    {
        print("save");
        SettingsInfo saveData = new SettingsInfo();
        saveData.soundVolume = soundVolume;
        saveData.musicVolume = musicVolume;
        saveData.playerStyle = playerStyle;
        print(saveData.soundVolume);
        print(saveData.musicVolume);
        ES3.Save("Settings", saveData);
    }

    public void LoadSettings()
    {
        if (ES3.KeyExists("Settings"))
        {
            SettingsInfo loadData = ES3.Load<SettingsInfo>("Settings");
            soundVolume = loadData.soundVolume;
            musicVolume = loadData.musicVolume;
            playerStyle = loadData.playerStyle;

            SFXManager.main.UpdateVolume();
            MusicManager.main.UpdateMusicVolume();
            MenuManager.main.SetPlayerStyle(playerStyle);
        }
        else
        {
            SaveSettings();

            SFXManager.main.UpdateVolume();
            MusicManager.main.UpdateMusicVolume();
        }
    }
}

[System.Serializable]
public class SettingsInfo
{
    public float soundVolume;
    public float musicVolume;
    public PlayerStyle playerStyle;

}
