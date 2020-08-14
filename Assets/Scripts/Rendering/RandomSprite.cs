using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RandomSprite : MonoBehaviour
{
    public bool stayInPlace = false;
    public float duration = 1f;
    [SerializeField] private Sprite[] frameArray;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Vector3 localPosition;


    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        localPosition = transform.TransformPoint(transform.localPosition);
    }
    void OnEnable()
    {
        startingPosition = new Vector3(transform.parent.position.x, localPosition.y, 0);
        startingRotation = transform.parent.rotation;
        if (frameArray.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        int randomIndex = Random.Range(0, frameArray.Length - 1);
        spriteRenderer.sprite = frameArray[randomIndex];
        StartCoroutine(Deactivate());
    }

    void Update()
    {
        if (stayInPlace)
        {
            transform.position = startingPosition;
            transform.rotation = startingRotation;
        }
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
