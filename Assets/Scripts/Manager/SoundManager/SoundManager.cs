using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource se;
    [SerializeField] private AudioClip gameStartSe;
    [SerializeField] private AudioClip decideSe;
    [SerializeField] private AudioClip cancelSe;
    [SerializeField] private float low = 0.95f;
    [SerializeField] private float high = 1.05f;

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

    public void PlaySingle(AudioClip clip)
    {
        se.clip = clip;
        se.Play();
    }

    public void RandomSe(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(low, high);
        se.pitch = randomPitch;
        se.clip = clips[randomIndex];
        se.Play();
    }
}