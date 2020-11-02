using System.Collections;
using UnityEngine;

public class MusicMixer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] menuMusicClips;
    [Range(0f, 5f)]
    public float menuMusicVolume = 1f;
    public AudioClip[] gameMusicClips;
    [Range(0f, 5f)]
    public float gameMusicVolume = 1f;

    bool isOnMenu = false;
    bool isInGame = false;
    bool musicChange = false;
    AudioClip nextMusicClip;

    private void Start()
    {
        Cursor.visible = false;
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.position = Camera.main.transform.position;
    }

    public void MenuMusic()
    {
        if(isOnMenu)
        {
            return;
        }
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        audioSource = GetComponent<AudioSource>();
        int index = Random.Range(0, menuMusicClips.Length);
        audioSource.clip = menuMusicClips[index];
        audioSource.volume = menuMusicVolume * musicVolume / 100;
        audioSource.Play();
        isOnMenu = true;
        isInGame = false;
    }

    public void GameMusic()
    {
        if (isInGame)
        {
            return;
        }
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = gameMusicClips[0];
        audioSource.volume = gameMusicVolume * musicVolume / 100;
        audioSource.Play();
        isOnMenu = false;
        isInGame = true;
    }

    public void SwitchMusic(int score)
    {
        nextMusicClip = gameMusicClips[Mathf.FloorToInt(score / 2)];
        Debug.Log(nextMusicClip);
        if (Mathf.FloorToInt(score / 2) < gameMusicClips.Length && !musicChange)
        {
            StartCoroutine(ChangeMusic());
            musicChange = true;
        }
    }

    IEnumerator ChangeMusic()
    {
        yield return StartCoroutine(WaitForRealSeconds(audioSource.clip.length - audioSource.time));
        audioSource.Stop();
        audioSource.clip = nextMusicClip;
        audioSource.Play();
        musicChange = false;
    }

    public void UpdateVolume() { 
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        if (isOnMenu)
        {
            audioSource.volume = menuMusicVolume * musicVolume / 100;
        } else if (isInGame)
        {
            audioSource.volume = gameMusicVolume * musicVolume / 100;
        }
    }

    IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }
}
