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

    private void Start()
    {
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
    }

    public void GameMusic()
    {
        if (isInGame)
        {
            return;
        }
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        audioSource = GetComponent<AudioSource>();
        int index = Random.Range(0, gameMusicClips.Length);
        audioSource.clip = gameMusicClips[index];
        audioSource.volume = gameMusicVolume * musicVolume / 100;
        audioSource.Play();
        isOnMenu = false;
        isInGame = true;
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
}
