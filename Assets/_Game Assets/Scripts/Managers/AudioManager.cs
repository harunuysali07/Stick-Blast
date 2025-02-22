using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip pop;
    
    private AudioSource _bgMusic, _standard, _increasing;

    [Header("Audio Mixer"), SerializeField]
    private AudioMixer masterMixer;

    private AudioMixerGroup _musicGroup;
    private AudioMixerGroup _soundGroup;
    
    public AudioManager Initialize()
    {
        var mainCameraGameObject = GameManager.Instance.cameraManager.mainCameraBrain.gameObject;
        
        _bgMusic = mainCameraGameObject.AddComponent<AudioSource>();
        _standard = mainCameraGameObject.AddComponent<AudioSource>();
        _increasing = mainCameraGameObject.gameObject.AddComponent<AudioSource>();

        _musicGroup = masterMixer.FindMatchingGroups("Music")[0];
        _soundGroup = masterMixer.FindMatchingGroups("Sound Effects")[0];

        _bgMusic.outputAudioMixerGroup = _musicGroup;
        _standard.outputAudioMixerGroup = _soundGroup;
        _increasing.outputAudioMixerGroup = _soundGroup;

        UpdateAudioStates();
        
        return this;
    }

    public void UpdateAudioStates()
    {
        SetMusicState(DataManager.Music);
        SetSoundState(DataManager.Sound);
    }

    public void SetMusicState(bool state)
    {
        masterMixer.SetFloat("Music_Volume", state ? 0 : -80);
    }

    public void SetSoundState(bool state)
    {
        masterMixer.SetFloat("Sound_Volume", state ? 0 : -80);
    }

    public void PlayUIButtonClick()
    {
        Play(pop);
    }

    private void Play(AudioClip clip)
    {
        if (!DataManager.Sound)
            return;
        
        _standard.PlayOneShot(clip);
    }

    private void PlayWithPitch(AudioClip clip, float pitch)
    {
        if (!DataManager.Sound)
            return;
        
        _increasing.pitch = pitch;
        _increasing.PlayOneShot(clip);
    }
}