using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPartyTitle : MonoBehaviour
{
    public float _amplitude = 4000F;
    public float _frequency = 3F;
    public float _turnAmount = 0.08F;

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Mathf.Sin(Time.time) / _amplitude;
        float y = Mathf.Sin(Time.time) / _amplitude;
        float z = Mathf.Sin(Time.time * _frequency) * _turnAmount;

        transform.localScale = transform.localScale + new Vector3(x, y, 0);
        transform.Rotate(new Vector3(0, 0, z));
        
    }
}
