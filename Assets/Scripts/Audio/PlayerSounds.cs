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
    public AudioClip[] smashServiceClips;
    [Range(0f, 5f)]
    public float smashServiceVolume = 1f;

    public void DashSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, dashClips.Length);
        audioSource.PlayOneShot(dashClips[index], dashVolume * sfxVolume / 100);
    }

    public void JumpSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, jumpClips.Length);
        audioSource.PlayOneShot(jumpClips[index], jumpVolume * sfxVolume / 100);
    }

    public void BumpSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, bumpClips.Length);
        audioSource.PlayOneShot(bumpClips[index], bumpVolume * sfxVolume / 100);
    }

    public void BumpImpactSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, bumpImpactClips.Length);
        audioSource.PlayOneShot(bumpImpactClips[index], bumpImpactVolume * sfxVolume / 100);
    }

    public void BallContactSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, ballContactClips.Length);
        audioSource.PlayOneShot(ballContactClips[index], ballContactVolume * sfxVolume / 100);
    }

    public void ChargeJumpSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, chargeJumpClips.Length);
        audioSource.PlayOneShot(chargeJumpClips[index], chargeJumpVolume * sfxVolume / 100);
    }

    public void SmashSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, smashClips.Length);
        audioSource.PlayOneShot(smashClips[index], smashVolume * sfxVolume / 100);
    }

    public void SmashImpactSound(float intensity)
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, smashImpactClips.Length);
        audioSource.PlayOneShot(smashImpactClips[index], smashImpactVolume * intensity * sfxVolume / 100);
    }

    public void SmashServiceSound()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        int index = Random.Range(0, smashServiceClips.Length);
        audioSource.PlayOneShot(smashServiceClips[index], smashServiceVolume * sfxVolume / 100);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
