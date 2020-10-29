using System.Collections;
using UnityEngine;

public class MessageAnimation : MonoBehaviour
{
    public Vector3 startingPosition = new Vector3(30, -4.5f, 0);
    public Vector3 middleFirstPosition = new Vector3(3, -4.5f, 0);
    public Vector3 middleLastPosition = new Vector3(-3, -4.5f, 0);
    public Vector3 endingPosition = new Vector3(-30, -4.5f, 0);

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
            transform.position = Vector3.Lerp(startingPosition, middleFirstPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(middleFirstPosition, middleLastPosition,  t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < duration)
        {
            transform.position = Vector3.Lerp(middleLastPosition, endingPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
