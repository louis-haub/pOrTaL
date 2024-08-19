using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public string music = "event:/PortalGun";
    FMOD.Studio.EventInstance gunEvent;
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        FMODUnity.RuntimeManager.CoreSystem.mixerResume();
        gunEvent = FMODUnity.RuntimeManager.CreateInstance(music);
        gunEvent.setParameterByName("Size", 1);
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
}

