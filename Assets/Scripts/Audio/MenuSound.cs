using UnityEngine;

public class MenuSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] selectClips;
    [Range(0f, 5f)]
    public float selectVolume = 1f;
    public AudioClip[] confirmClips;
    [Range(0f, 5f)]
    public float confirmVolume = 1f;
    public AudioClip[] cancelClips;
    [Range(0f, 5f)]
    public float cancelVolume = 1f;
    public AudioClip[] appearClips;
    [Range(0f, 5f)]
    public float appearVolume = 1f;
    public AudioClip[] appearEndClips;
    [Range(0f, 5f)]
    public float appearEndVolume = 1f;

    public void SelectSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, selectClips.Length);
        audioSource.PlayOneShot(selectClips[index], selectVolume * sfxVolume / 100);
    }

    public void ConfirmSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, confirmClips.Length);
        audioSource.PlayOneShot(confirmClips[index], confirmVolume * sfxVolume / 100);
    }

    public void CancelSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, cancelClips.Length);
        audioSource.PlayOneShot(cancelClips[index], cancelVolume * sfxVolume / 100);
    }

    public void AppearSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, appearClips.Length);
        audioSource.PlayOneShot(appearClips[index], appearVolume * sfxVolume / 100);
    }
    
    public void AppearEndSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, appearEndClips.Length);
        audioSource.PlayOneShot(appearEndClips[index], appearEndVolume * sfxVolume / 100);
    }

    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }
}
