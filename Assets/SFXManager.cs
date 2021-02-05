using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager main;

    public float masterVolume;
    public AudioSource[] shortAudioSources;
        int shortSrcIndex;
    [Space]
    [Header("Game Sounds")]
    public AudioClip[] playerHit;
    public Vector4 remapPlayerHitVol;
    public AudioClip[] crateHit;
    public Vector4 remapCrateHitVol;
    public AudioClip[] crateBreak;
    [Space]
    public AudioClip buttonOn;
    public AudioClip buttonOff;
    public AudioClip tntExplode;

    public enum SelectSound
    {
        DefaultClick,
        HardClick,
        GoBack
    }
    [Header("UI Sounds")]
    public AudioClip[] selectSounds;

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


    public void PlaySelectSound(SelectSound selectSound, float vol)
    {
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(selectSounds[(int)selectSound], vol*.6f, .05f);
    }

    public void CrateBreakSound()
    {
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(crateBreak[Random.Range(0,crateBreak.Length)], .75f, .05f);
    }

    public void PlayerHitSound(float magnitude)
    {
        float targetVolume = magnitude.Remap(remapPlayerHitVol.x, remapPlayerHitVol.y, remapPlayerHitVol.z, remapPlayerHitVol.w);
        targetVolume = Mathf.Clamp(targetVolume, remapPlayerHitVol.z, remapPlayerHitVol.w);
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(playerHit[Random.Range(0,playerHit.Length)], targetVolume, 0,1,.07f);
    }

    public void CrateHitSound(float magnitude)
    {
        float targetVolume = magnitude.Remap(remapCrateHitVol.x, remapCrateHitVol.y, remapCrateHitVol.z, remapCrateHitVol.w);

        targetVolume = Mathf.Clamp(targetVolume, remapCrateHitVol.z, remapCrateHitVol.w);
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(crateHit[Random.Range(0, crateHit.Length)], targetVolume, 0, 1, .07f);
    }

    public void TNTExplode()
    {
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(tntExplode, .7f, 0, 1, .07f);

    }

    public void ButtonOnOff(bool turnedOn)
    {
        shortSrcIndex = (shortSrcIndex + 1) % shortAudioSources.Length;
        shortAudioSources[shortSrcIndex].PlaySound(turnedOn ? buttonOn : buttonOff, 1f, 0, 1, .07f);
    }

    public void UpdateVolume()
    {
        masterVolume = SettingsManager.main.soundVolume;
    }

}
