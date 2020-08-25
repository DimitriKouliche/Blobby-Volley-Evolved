using UnityEngine;

public class RandomSoundOnEnable : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] clips;
    public float volume = 1f;

    private void OnEnable()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index], volume);
    }

}
