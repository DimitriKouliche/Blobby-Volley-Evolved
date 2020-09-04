using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour
{
    GameObject canonLeft;
    GameObject canonRight;
    Vector3 canonLeftPosition = new Vector3(-12, -5, 2);
    Vector3 canonRightPosition = new Vector3(12, -5, 2);
    Vector3 canonLeftInitialPosition = new Vector3(-20, -12, 2);
    Vector3 canonRightInitialPosition = new Vector3(20, -12, 2);

    // Start is called before the first frame update
    void Start()
    {
        canonLeft = FindChild(gameObject, "CanonLeft");
        canonRight = FindChild(gameObject, "CanonRight");
        StartCoroutine(MainAnimation(1));
    }

    IEnumerator MainAnimation(float duration)
    {
        float t = 0.0f;
        while (t < duration)
        {
            canonLeft.transform.position = Vector3.Lerp(canonLeftInitialPosition, canonLeftPosition, t / duration);
            canonRight.transform.position = Vector3.Lerp(canonRightInitialPosition, canonRightPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        FindChild(canonLeft, "Shot").SetActive(true);
        FindChild(canonRight, "Shot").SetActive(true);
        t = 0.0f;
        while (t < duration/5)
        {
            canonLeft.transform.position = Vector3.Lerp(canonLeftPosition, canonLeftInitialPosition, t / duration);
            canonRight.transform.position = Vector3.Lerp(canonRightPosition, canonRightInitialPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < duration/5)
        {
            canonLeft.transform.position = Vector3.Lerp(canonLeftInitialPosition, canonLeftPosition, (4*duration/5 + t) / duration);
            canonRight.transform.position = Vector3.Lerp(canonRightInitialPosition, canonRightPosition, (4 * duration / 5 + t) / duration);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        t = 0.0f;
        while (t < duration)
        {
            canonLeft.transform.position = Vector3.Lerp(canonLeftPosition, canonLeftInitialPosition, t / duration);
            canonRight.transform.position = Vector3.Lerp(canonRightPosition, canonRightInitialPosition, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }

    GameObject FindChild(GameObject parent, string name)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
