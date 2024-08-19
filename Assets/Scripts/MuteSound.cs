using UnityEngine;
using FMODUnity;

public class MuteSound : MonoBehaviour
{
    public const string SoundPrefsKey = "ktb_sound";

    public GameObject soundOnButton;
    public GameObject soundOffButton;

    private FMOD.Studio.VCA _mute;
    private bool _isMuted = false;

    private void Start()
    {
        _mute = RuntimeManager.GetVCA("vca:/MUTE");

        var soundEnabled = PlayerPrefs.GetInt(SoundPrefsKey, 1);
        if (soundEnabled < 1)
        {
            MuteAudio();
        }
        else
        {
            ApplyButtonState();
        }
    }

    public void MuteAudio()
    {
        if (!_isMuted)
        {
            _mute.setVolume(0);
            _isMuted = true;
            PlayerPrefs.SetInt(SoundPrefsKey, 0);
        }
        else
        {
            _mute.setVolume(1);
            _isMuted = false;
            PlayerPrefs.SetInt(SoundPrefsKey, 1);
        }

        ApplyButtonState();
    }

    private void ApplyButtonState()
    {
        soundOnButton.SetActive(_isMuted);
        soundOffButton.SetActive(!_isMuted);
    }
}
