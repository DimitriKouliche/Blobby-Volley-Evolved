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


    void OnEnable()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        if (frameArray.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
