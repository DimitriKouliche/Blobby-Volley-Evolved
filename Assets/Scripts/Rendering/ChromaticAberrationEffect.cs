using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationEffect : MonoBehaviour
{
    public Volume volume;
    public float SlowMo = 0.5f;
    ChromaticAberration chromatic;
   
    void Start()
    {
        volume.profile.TryGet<ChromaticAberration>(out chromatic);
        StartCoroutine(SmashEffect());
    }

    IEnumerator SmashEffect()
    {
        float timer = 0f;
        float time = 0.69f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            chromatic.intensity.value = Mathf.PingPong(Time.time * 3f, 1f);
            Time.timeScale = SlowMo;
            yield return null;
        }
        chromatic.intensity.value = 0f;
        Time.timeScale = 1f;
    }
}
