using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] frameArray;
    private int currentFrame;
    private float timer;
    private float framerate = .1f;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(currentFrame >= frameArray.Length -1)
        {
            currentFrame = 0;
            gameObject.SetActive(false);
        }
        timer += Time.deltaTime;
        if (timer >= framerate)
        {
            timer -= framerate;
            currentFrame = (currentFrame + 1);
            spriteRenderer.sprite = frameArray[currentFrame];
        }

    }
}
