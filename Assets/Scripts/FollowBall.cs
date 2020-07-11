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
        if(heading.x < -1)
        {
            heading.x = -1;
        }
        if (heading.y > 1)
        {
            heading.y = 1;
        }
        if (heading.y < -1)
        {
            heading.y = -1;
        }
        transform.localPosition = new Vector3(heading.x * 0.05f, heading.y * 0.05f - 0.05f, -3);
    }
}
