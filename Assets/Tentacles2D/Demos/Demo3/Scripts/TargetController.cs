using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class TargetController : MonoBehaviour
    {
        [SerializeField] private float drag = .5f;
        private float angle, z;
        private new SpriteRenderer renderer;

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            z = transform.position.z;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z = 15f;
                var position = Vector2.Lerp(transform.position, Camera.main.ScreenToWorldPoint(mousePosition), .1f);
                transform.position = new Vector3(position.x, position.y, z);
            }
            else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (renderer.isVisible)
                {
                    transform.position += new Vector3(Input.GetAxis("Horizontal") * drag, Input.GetAxis("Vertical") * drag, 0);
                }
                else
                {
                    transform.position = new Vector3(0, 0, z);
                }
            }
        }
    }
}