using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Ins
    {
        get
        {
            m_ins = FindObjectOfType<AudioManager>();
            if (!m_ins) m_ins = new GameObject().AddComponent<AudioManager>();
            return m_ins;
        }
    }
    
    [Serializable]
    private class Sound
    {
        public string name;
        public AudioClip obj;
    }
    
    [SerializeField] private Sound[] _sounds;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    private static AudioManager m_ins;

    private void Start()
    {
        PlayAudio(ValuesConst.AUDIO_MUSIC,false);
        if (_musicSource)
        {
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
        }

        if (_sfxSource)
        {
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
        }
    }

    public void PlayAudio(string name, bool isSfx = true)
    {
        Sound sound = Array.Find(_sounds, s => s.name.Equals(name));
        if (sound == null) return;
        if (isSfx && _sfxSource) _sfxSource.PlayOneShot(sound.obj);
        else if (!isSfx &&_musicSource)
        {
            _musicSource.clip = sound.obj;
            _musicSource.Play();
        }
    }

    public void PauseMusic(bool isPause = true)
    {
        if (!_musicSource) return;
        if(isPause) _musicSource.Pause();
        else _musicSource.Play();
    }

    public void AudioVol(float vol, bool isSfx = true)
    {
        if (isSfx && _sfxSource) _sfxSource.volume = vol;
        else if (!isSfx &&_musicSource) _musicSource.volume = vol;
    }
    
}
