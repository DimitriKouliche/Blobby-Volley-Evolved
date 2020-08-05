using System.Collections;
using UnityEngine;

public class EyeLogics : MonoBehaviour
{
    public GameObject ball;

    Vector3 heading;

    // Update is called once per frame
    void Update()
    {
        if(ball == null)
        {
            return;
        }
        heading = ball.transform.position - transform.position;
        if (heading.x > 1)
        {
            heading.x = 1;
        }
        if(heading.x < 0)
        {
            heading.x = 0;
        }
        if (heading.y > 1)
        {
            heading.y = 1;
        }
        if (heading.y < 0)
        {
            heading.y = 0;
        }
        transform.localPosition = new Vector3(heading.x * 0.2f-0.15f, heading.y * 0.2f + 0.5f, -3);
    }

    public void ChangeEyeColor(float intensity, Color startColor, Color endColor)
    {
        GetComponent<SpriteRenderer>().color = Color.Lerp(startColor, endColor, intensity);
    }
}
