using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAbberationEffect : MonoBehaviour
{
    public Volume volume;
    ChromaticAberration chromatic;
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet<ChromaticAberration>(out chromatic);
    }

    // Update is called once per frame
    void Update()
    {
        chromatic.intensity.value = Mathf.PingPong(Time.time * 6, 1f);
    }
}
