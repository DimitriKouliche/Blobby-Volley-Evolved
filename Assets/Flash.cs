using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(FlashAnim());
    }

    void OnDisable()
    {
        spriteRenderer.color = new Color(79, 79, 79, 0);
    }

    IEnumerator FlashAnim()
    {
        float timer = 0f;
        float time = 0.4f;
        float maxAlpha = 170f;
        float alphaBlack;

        while (timer < time)
        {
            alphaBlack = Mathf.PingPong(Time.time, maxAlpha);
            spriteRenderer.color = new Color(79, 79, 79, alphaBlack);
        }

        StopCoroutine(FlashAnim());
        yield return null;
    }
}
