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
    public AudioClip[] victoryMusicClips;
    [Range(0f, 5f)]
    public float victoryMusicVolume = 1f;
    public AudioClip fadeOutClip;
    [Range(0f, 5f)]
    public float fadeOutVolume = 1f;
    public bool gameOver = false;

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
        audioSource.loop = true;
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
        audioSource.loop = true;
    }

    public void VictoryMusic()
    {
        if (isOnMenu || audioSource.clip == victoryMusicClips[0])
        {
            return;
        }
        GameObject.Find("Cup").GetComponent<AudioSource>().Stop();
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = victoryMusicClips[0];
        audioSource.volume = victoryMusicVolume * musicVolume / 100;
        audioSource.Play();
        isOnMenu = false;
        isInGame = true;
        audioSource.loop = false;
    }

    public void StopMusic()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        audioSource.volume = fadeOutVolume * musicVolume / 100;
        audioSource.clip = fadeOutClip;
        audioSource.Play();
        audioSource.loop = false;
    }

    public void SwitchMusic(int score)
    {
        nextMusicClip = gameMusicClips[Mathf.FloorToInt(score / 2)];
        if (Mathf.FloorToInt(score / 2) < gameMusicClips.Length && !musicChange)
        {
            StartCoroutine(ChangeMusic());
            musicChange = true;
        }
    }
    public void RestartGameMusic()
    {
        audioSource.Stop();
        audioSource.clip = gameMusicClips[0];
        audioSource.loop = true;
        audioSource.Play();
    }

    IEnumerator ChangeMusic()
    {
        yield return StartCoroutine(WaitForRealSeconds(audioSource.clip.length - audioSource.time));
        if(gameOver)
        {
            gameOver = false;
            yield break;
        }
        audioSource.Stop();
        audioSource.clip = nextMusicClip;
        audioSource.Play();
        audioSource.loop = true;
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
