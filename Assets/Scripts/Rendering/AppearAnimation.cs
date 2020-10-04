using System.Collections;
using UnityEngine;

public class AppearAnimation : MonoBehaviour
{
    public int maxPlayers = 2;

    void Start()
    {
        if(maxPlayers == 2)
        {
            StartCoroutine(AppearCoroutine2Players());
        }
        if(maxPlayers == 4)
        {
            StartCoroutine(AppearCoroutine4Players());
        }
    }

    IEnumerator AppearCoroutine2Players()
    {
        float t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(-4.2f, 9.25f, -0.5f), new Vector3(-1.9f, 0.1f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection1").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(-0.03f, 10.39f, -0.5f), new Vector3(2.6f, -0.1f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection2").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        var objects = GameObject.FindGameObjectsWithTag("MenuText");
        t = 0.0f;
        while (t < 0.2f)
        {
            float alpha = Mathf.Lerp(0f, 1f, t * 5);
            foreach (var obj in objects)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, alpha);
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator AppearCoroutine4Players()
    {
        float t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(-6.94f, 9.95f, -0.5f), new Vector3(-4.6f, 0.7f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection1").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(-3.8f, 9.89f, -0.5f), new Vector3(-1.4f, 0.5f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection2").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(-0.71f, 9.95f, -0.5f), new Vector3(1.7f, 0.5f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection3").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        t = 0.0f;
        while (t < 0.2f)
        {
            Vector3 position = Vector3.Lerp(new Vector3(2.39f, 9.89f, -0.5f), new Vector3(4.69f, 0.87f, -0.5f), t * 5);
            FindChild(gameObject, "FondSelection4").transform.localPosition = position;
            t += Time.deltaTime;
            yield return null;
        }
        var objects = GameObject.FindGameObjectsWithTag("MenuText");
        t = 0.0f;
        while (t < 0.2f)
        {
            float alpha = Mathf.Lerp(0f, 1f, t * 5);
            foreach (var obj in objects)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, alpha);
            }
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
