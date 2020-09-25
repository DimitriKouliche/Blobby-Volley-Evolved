using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] dashClips;
    [Range(0f, 5f)]
    public float dashVolume = 1f;
    public AudioClip[] jumpClips;
    [Range(0f, 5f)]
    public float jumpVolume = 1f;
    public AudioClip[] bumpClips;
    [Range(0f, 5f)]
    public float bumpVolume = 1f;
    public AudioClip[] bumpImpactClips;
    [Range(0f, 5f)]
    public float bumpImpactVolume = 1f;
    public AudioClip[] ballContactClips;
    [Range(0f, 5f)]
    public float ballContactVolume = 1f;
    public AudioClip[] chargeJumpClips;
    [Range(0f, 5f)]
    public float chargeJumpVolume = 1f;
    public AudioClip[] smashClips;
    [Range(0f, 5f)]
    public float smashVolume = 1f;
    public AudioClip[] smashImpactClips;
    [Range(0f, 5f)]
    public float smashImpactVolume = 1f;

    public void DashSound()
    {
        int index = Random.Range(0, dashClips.Length);
        audioSource.PlayOneShot(dashClips[index], dashVolume);
    }

    public void JumpSound()
    {
        int index = Random.Range(0, jumpClips.Length);
        audioSource.PlayOneShot(jumpClips[index], jumpVolume);
    }

    public void BumpSound()
    {
        int index = Random.Range(0, bumpClips.Length);
        audioSource.PlayOneShot(bumpClips[index], bumpVolume);
    }

    public void BumpImpactSound()
    {
        int index = Random.Range(0, bumpImpactClips.Length);
        audioSource.PlayOneShot(bumpImpactClips[index], bumpImpactVolume);
    }

    public void BallContactSound()
    {
        int index = Random.Range(0, ballContactClips.Length);
        audioSource.PlayOneShot(ballContactClips[index], ballContactVolume);
    }

    public void ChargeJumpSound()
    {
        int index = Random.Range(0, chargeJumpClips.Length);
        audioSource.PlayOneShot(chargeJumpClips[index], chargeJumpVolume);
    }

    public void SmashSound()
    {
        int index = Random.Range(0, smashClips.Length);
        audioSource.PlayOneShot(smashClips[index], smashVolume);
    }

    public void SmashImpactSound(float intensity)
    {
        int index = Random.Range(0, smashImpactClips.Length);
        audioSource.PlayOneShot(smashImpactClips[index], smashImpactVolume * intensity);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
