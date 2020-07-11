using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public GameObject ball;

    Vector3 heading;

    // Update is called once per frame
    void Update()
    {
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
        transform.localPosition = new Vector3(heading.x * 0.005f, heading.y * 0.006f - 0.006f, -3);
    }
}
