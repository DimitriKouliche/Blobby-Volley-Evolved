using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject[] layers;
    public float parallaxStrength = 0.1f;

    void Start()
    {
        UpdateLayers();
    }

    void Update()
    {
        UpdateLayers();
    }

    void UpdateLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            Transform t = layers[layers.Length - i - 1].transform;
            t.position = new Vector3(-i * transform.position.x * parallaxStrength, t.position.y, t.position.z);
        }
    }
}
