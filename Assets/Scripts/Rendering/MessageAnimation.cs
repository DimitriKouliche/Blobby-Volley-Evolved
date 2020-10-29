using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PassByAnimation(0.3f));
    }

    IEnumerator PassByAnimation(float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(new Vector3(30, -4.5f, 0), new Vector3(3, -4.5f, 0), t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(new Vector3(3, -4.5f, 0), new Vector3(-3, -4.5f, 0),  t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(new Vector3(-3, -4.5f, 0), new Vector3(-30, -4.5f, 0), t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
