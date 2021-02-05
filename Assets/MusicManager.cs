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
    public AudioClip iceAudio;

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
        if (cScene < 10 || cScene == 14)
        {
            targetAudio = grassAudio;
        }
        else
        {
            targetAudio = iceAudio;
        }
        if (aSrc.clip != targetAudio)
        {
            aSrc.volume -= Time.deltaTime/6f;
            if (aSrc.volume <= .01f)
            {
                aSrc.clip = targetAudio;
                aSrc.Stop();
                aSrc.Play();
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
}
