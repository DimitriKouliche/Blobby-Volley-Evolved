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
        int index = Random.Range(0, selectClips.Length);
        audioSource.PlayOneShot(selectClips[index], selectVolume);
    }

    public void ConfirmSound()
    {
        int index = Random.Range(0, confirmClips.Length);
        audioSource.PlayOneShot(confirmClips[index], confirmVolume);
    }

    public void CancelSound()
    {
        int index = Random.Range(0, cancelClips.Length);
        audioSource.PlayOneShot(cancelClips[index], cancelVolume);
    }

    public void AppearSound()
    {
        int index = Random.Range(0, appearClips.Length);
        audioSource.PlayOneShot(appearClips[index], appearVolume);
    }
    
    public void AppearEndSound()
    {
        int index = Random.Range(0, appearEndClips.Length);
        audioSource.PlayOneShot(appearEndClips[index], appearEndVolume);
    }

    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }
}
