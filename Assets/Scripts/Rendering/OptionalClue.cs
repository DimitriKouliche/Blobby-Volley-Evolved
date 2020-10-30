using System.Collections;
using UnityEngine;

public class OptionalClue : MonoBehaviour
{
    public float delay = 8f;
    bool fadeAway = false;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        StartCoroutine(FadeInFadeOut());
    }

    void OnDisable()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }

    IEnumerator FadeInFadeOut()
    {
        yield return new WaitForSeconds(delay);
        while (gameObject.activeSelf)
        {
            if (fadeAway)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    spriteRenderer.color = new Color(1, 1, 1, i);
                    yield return null;
                }
                fadeAway = false;
            }
            else
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    spriteRenderer.color = new Color(1, 1, 1, i);
                    yield return null;
                }
                fadeAway = true;
            }
        }
    }
}
