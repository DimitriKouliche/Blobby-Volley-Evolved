using System.Collections;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public bool stayInPlace = false;
    public float duration = 1f;
    [SerializeField] private Sprite[] frameArray;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        int randomIndex = Random.Range(0, frameArray.Length - 1);
        spriteRenderer.sprite = frameArray[randomIndex];
        StartCoroutine(Deactivate());
    }

    void OnEnable()
    {
        startingPosition = transform.parent.position;
        startingRotation = transform.parent.rotation;
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
