using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public bool stayInPlace = false;
    public float framerate = .1f;
    public bool loop;
    [SerializeField] private Sprite[] frameArray;
    private int currentFrame;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        startingPosition = transform.parent.position;
        startingRotation = transform.parent.rotation;
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
            transform.rotation = startingRotation;
        }

    }
}
