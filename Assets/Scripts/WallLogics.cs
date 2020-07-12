using System.Collections;
using UnityEngine;

public class WallLogics : MonoBehaviour
{
    bool isScaling;

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.name == "Blob 1(Clone)" || collision.gameObject.name == "Blob 2(Clone)")
        {
            StartCoroutine(SquashAnimation(collision.gameObject.transform,
                new Vector3(collision.gameObject.transform.localScale.x * Mathf.Min(1, (1 - collision.relativeVelocity.x / 30)), 
                collision.gameObject.transform.localScale.y * Mathf.Min(1, (1 - collision.relativeVelocity.y / 30)), 
                collision.gameObject.transform.localScale.z),
                0.3f));
        }
    }

    IEnumerator SquashAnimation(Transform objectToScale, Vector3 toScale, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isScaling)
        {
            yield break; ///exit if this is still running
        }
        isScaling = true;

        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;


        while (counter < duration / 2)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;
            objectToScale.localScale = Vector3.Lerp(toScale, startScaleSize, counter / duration);
            yield return null;
        }

        isScaling = false;
    }
}
