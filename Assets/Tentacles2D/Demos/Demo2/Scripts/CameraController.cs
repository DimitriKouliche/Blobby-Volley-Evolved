using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        private void Update()
        {
            if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
            {
                var position = Vector2.Lerp(transform.position, (Vector2)transform.position + new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed, .2f);
                transform.position = new Vector3(position.x, position.y, -10f);
            }
        }
    }
}