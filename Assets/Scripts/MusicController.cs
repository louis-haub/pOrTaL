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
    public string jumpInstance = "event:/Jump";
    public string landInstance = "event:/Land";
    FMOD.Studio.EventInstance stepEvent;
    FMOD.Studio.EventInstance gunEvent;
    FMOD.Studio.EventInstance musicEvent;
    FMOD.Studio.EventInstance landEvent;
    FMOD.Studio.EventInstance jumpEvent;
    public bool playingFootsteps = false;
    private float footstepDistanceTime = 0.5f;

    public float randomActionTime = 20f;
    private float size = 1f;
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        
        musicEvent = FMODUnity.RuntimeManager.CreateInstance(musicInstance);
        musicEvent.setParameterByName("Action", 0);
        musicEvent.start();
        
        StartCoroutine(musicSetter());
        StartCoroutine(footsteps());
        
        
    }
    public void triggerGunAudio()
    {
        gunEvent = FMODUnity.RuntimeManager.CreateInstance(gunInstance);
        gunEvent.setParameterByName("Size", size);
        gunEvent.start();
        gunEvent.release();
    }

    public void triggerFootstep()
    {
        stepEvent = FMODUnity.RuntimeManager.CreateInstance(stepInstance);
        stepEvent.start();
        stepEvent.release();
    }

    public void triggerLanding()
    {
        landEvent = FMODUnity.RuntimeManager.CreateInstance(landInstance);
        landEvent.setParameterByName("Size", size);
        landEvent.start();
        landEvent.release();

    }


    public void triggerJump()
    {
        jumpEvent = FMODUnity.RuntimeManager.CreateInstance(jumpInstance);
        gunEvent.setParameterByName("Size", size);
        jumpEvent.start();
        jumpEvent.release();
    }
    public void setSize(float progress)
    {
        size = progress;
        
    }
    IEnumerator footsteps()
    {
        while (true)
        {
            //Debug.Log("playing Footsteps: "+ playingFootsteps);
            if (playingFootsteps)
            {
                triggerFootstep();
            }
            yield return new WaitForSeconds(footstepDistanceTime);
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

