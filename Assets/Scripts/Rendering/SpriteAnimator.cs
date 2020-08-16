using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SpriteAnimator : MonoBehaviour
{
    public bool stayInPlace = false;
    public float framerate = .1f;
    public bool loop;
    public bool rotate = false;
    [SerializeField] private Sprite[] frameArray;
    private int currentFrame = 0;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Vector3 localPosition;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        localPosition = transform.localPosition;
    }

    void OnEnable()
    {
        if(transform.parent != null)
        {
            startingPosition = transform.parent.position;
            startingRotation = transform.parent.rotation;
        } else
        {
            startingPosition = transform.position;
            startingRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (currentFrame >= frameArray.Length)
        {
            currentFrame = 0;
            timer = 0f;
            spriteRenderer.sprite = frameArray[0];
            if (!loop)
            {
                gameObject.SetActive(false);
            }
        }

        timer += Time.deltaTime;

        if (timer >= framerate)
        {
            timer -= framerate;
            currentFrame = (currentFrame + 1);
            if(currentFrame < frameArray.Length)
                spriteRenderer.sprite = frameArray[currentFrame];
        }

        if (stayInPlace)
        {
            transform.position = startingPosition;
        }

        if(!rotate)
        {
            transform.rotation = startingRotation;
        }

    }
}
