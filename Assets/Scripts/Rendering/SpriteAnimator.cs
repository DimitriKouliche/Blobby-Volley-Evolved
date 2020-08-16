using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SpriteAnimator : MonoBehaviour
{
    public bool stayInPlace = false;
    public float framerate = .1f;
    public bool loop;
    public bool rotate = false;
    [SerializeField] private Sprite[] frameArray;
    private int currentFrame;
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
        if (currentFrame >= frameArray.Length - 1)
        {
            currentFrame = 0;
            if(!loop)
            {
                gameObject.SetActive(false);
            }
        }

        timer += Time.deltaTime;

        if (timer >= framerate)
        {
            timer -= framerate;
            currentFrame = (currentFrame + 1);
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
