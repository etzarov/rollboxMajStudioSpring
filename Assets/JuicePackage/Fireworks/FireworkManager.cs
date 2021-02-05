using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkManager : MonoBehaviour
{
    public static FireworkManager main;


    private void Awake()
    {
        main = this;
    }

    public ParticleSystem launchPS;
    ParticleSystem.MainModule mainModule;
    int pCount;
    [Space]
    public AudioSource[] audioSrcs;
    int srcIndex;
    [Space]
    public AudioClip[] launchClips;
    public AudioClip[] explodeClips;

    

    // Start is called before the first frame update
    void Start()
    {
        mainModule = launchPS.main;
    }

    // Update is called once per frame
    void Update()
    {
            
            int _count = launchPS.particleCount;
            if (_count < pCount)
            { //particle has died
                PlayExplodeSound();
            }
            else if (_count > pCount)
            { //particle has been born
                PlayLaunchSound();
            }

            pCount = _count;

    }

    public void LaunchParticles()
    {
        if (!launchPS.isPlaying)
        {
            launchPS.Play();
        }
    }

    public void StopParticles()
    {
        if (launchPS.isPlaying)
        {
            launchPS.Stop();
            launchPS.Clear();
        }
    }

    public void PlayLaunchSound()
    {
        srcIndex = (srcIndex + 1) % audioSrcs.Length;
        audioSrcs[srcIndex].PlaySound(launchClips[Random.Range(0, launchClips.Length)], .1f, .05f, 1, .1f);

    }

    public void PlayExplodeSound()
    {
        srcIndex = (srcIndex + 1) % audioSrcs.Length;
        audioSrcs[srcIndex].PlaySound(explodeClips[Random.Range(0, explodeClips.Length)], .25f, .1f, 1, .1f);
        float offsetVal = Random.Range(0, .4f);
    }
}
