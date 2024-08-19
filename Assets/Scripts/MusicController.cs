using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.UI;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public string gunInstance = "event:/PortalGun";
    public string musicInstance = "event:/Music";
    public string stepInstance = "event:/Footsteps";
    FMOD.Studio.EventInstance stepEvent;
    FMOD.Studio.EventInstance gunEvent;
    FMOD.Studio.EventInstance musicEvent;
    public bool playingFootsteps = false;
    private float footstepDistanceTime = 0.5f;

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
        stepEvent = FMODUnity.RuntimeManager.CreateInstance(stepInstance);
        StartCoroutine(footsteps());
    }
    public void triggerGunAudio()
    {
        gunEvent.start();
        gunEvent.release();
    }

    public void triggerFootstep()
    {
        stepEvent.start();
        stepEvent.release();
    }
    public void setSize(float progress)
    {
        gunEvent.setParameterByName("Size", progress);
    }
    IEnumerator footsteps()
    {
        while (true)
        {
            if (playingFootsteps)
            {
                triggerFootstep();
                yield return new WaitForSeconds(footstepDistanceTime);

            }
            else
            {
                yield return new WaitForSeconds(footstepDistanceTime);
            }
        }
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

