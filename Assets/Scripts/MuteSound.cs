using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MuteSound : MonoBehaviour
{
    private FMOD.Studio.VCA _mute;
    private bool _isMuted = false;

    private void Start()
    {
        _mute = RuntimeManager.GetVCA("vca:/MUTE");
    }

    public void MuteAudio()
    {
        if (!_isMuted) 
        {
            _mute.setVolume(0);
            _isMuted = true;
        }
        else
        {
            _mute.setVolume(1);
            _isMuted = false;
        }
            

    }
}
