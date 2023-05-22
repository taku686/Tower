using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource seAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioClip gameStartSe;
    [SerializeField] private AudioClip decideSe;
    [SerializeField] private AudioClip cancelSe;
    [SerializeField] private AudioClip bgm;
    [SerializeField] private float low = 0.95f;
    [SerializeField] private float high = 1.05f;
    private const float StopVolume = 0.1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void GameStartSe()
    {
        PlaySingle(gameStartSe);
    }

    public void DecideSe()
    {
        PlaySingle(decideSe);
    }

    public void CancelSe()
    {
        PlaySingle(cancelSe);
    }

    public void SeChangeVolume(float volume)
    {
        if (Mathf.Abs(seAudioSource.volume - volume) < float.Epsilon)
        {
            return;
        }

        if (volume <= StopVolume)
        {
            bgmAudioSource.mute = true;
        }
        else
        {
            bgmAudioSource.mute = false;
            seAudioSource.volume = volume;
        }
    }

    public void BgmChangeVolume(float volume)
    {
        if (Mathf.Abs(bgmAudioSource.volume - volume) < float.Epsilon)
        {
            return;
        }

        if (volume <= StopVolume)
        {
            bgmAudioSource.mute = true;
        }
        else
        {
            bgmAudioSource.mute = false;
            bgmAudioSource.volume = volume;
        }
    }

    public void BgmPlay()
    {
        bgmAudioSource.clip = bgm;
        bgmAudioSource.Play();
    }

    private void PlaySingle(AudioClip clip)
    {
        seAudioSource.clip = clip;
        seAudioSource.Play();
    }

    public void RandomSe(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(low, high);
        seAudioSource.pitch = randomPitch;
        seAudioSource.clip = clips[randomIndex];
        seAudioSource.Play();
    }
}