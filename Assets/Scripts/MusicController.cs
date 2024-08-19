using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public string gunInstance = "event:/PortalGun";
    public string musicInstance = "event:/Music";
    FMOD.Studio.EventInstance gunEvent;
    FMOD.Studio.EventInstance musicEvent;
    public float randomActionTime = 20f;
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        gunEvent = FMODUnity.RuntimeManager.CreateInstance(gunInstance);
        gunEvent.setParameterByName("Size", 1);
        musicEvent = FMODUnity.RuntimeManager.CreateInstance(musicInstance);
        musicEvent.setParameterByName("Action", 0);
        musicEvent.start();
        StartCoroutine(musicSetter());
    }
    public void triggerGunAudio()
    {
        gunEvent.start();
        gunEvent.release();
    }
    public void setSize(float progress)
    {
        gunEvent.setParameterByName("Size", progress);
    }
    IEnumerator musicSetter()
    {
        while (true)
        {
            yield return new WaitForSeconds(randomActionTime);
            //code here will execute after 5 seconds
            float action = UnityEngine.Random.Range(0.0f, 1.0f);
            musicEvent.setParameterByName("Action", action);
        }

    }
}

