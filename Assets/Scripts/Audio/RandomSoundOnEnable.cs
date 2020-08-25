using UnityEngine;

public class RandomSoundOnEnable : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] clips;

    private void OnEnable()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        int index = Random.Range(0, clips.Length);
        audioSource.clip = clips[index];
        audioSource.Play();
    }

}
