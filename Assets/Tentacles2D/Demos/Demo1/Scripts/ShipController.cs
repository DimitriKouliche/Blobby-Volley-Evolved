using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float speed;
        private float movement;
        private Rigidbody2D ship;
        private Vector2 left, right;
        [SerializeField] private Transform reflection;
        private SpriteRenderer reflectionSprite;
        [SerializeField] private ButtonHighlight buttonLeft;
        [SerializeField] private ButtonHighlight buttonRight;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            ship = GetComponent<Rigidbody2D>();
            reflectionSprite = reflection.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            left = Camera.main.ViewportToWorldPoint(new Vector2(0, .65f)) + new Vector3(-3f, 0);
            right = Camera.main.ViewportToWorldPoint(new Vector2(1f, .65f)) + new Vector3(3f, 0);
            ship.MovePosition(left + new Vector2(1f, 0));
        }

        private void Update()
        {
            /* controls */
            if (Input.touchCount > 0)  // touch
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    var worldTouch = Camera.main.ScreenToWorldPoint(touch.position);
                    if (worldTouch.y > -7f)
                    {
                        if (worldTouch.x > 0)
                        {
                            movement = 1f;
                            buttonRight.HighlightUpdate();
                        }
                        else
                        {
                            movement = -1f;
                            buttonLeft.HighlightUpdate();
                        }
                    }
                }
            }
            else if (Input.GetMouseButton(0)) // mouse
            {
                var worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (worldMouse.y > -7f)
                {
                    if (worldMouse.x > 0)
                    {
                        movement = 1f;
                        buttonRight.HighlightUpdate();
                    }
                    else
                    {
                        movement = -1f;
                        buttonLeft.HighlightUpdate();
                    }
                }
            }
            else // keyboard
            {
                movement = Input.GetAxis("Horizontal");
                if (movement > 0)
                    buttonRight.HighlightUpdate();
                else if (movement < 0)
                    buttonLeft.HighlightUpdate();
                if (movement == 0) movement = .6f;
            }

            /* decoration */
            var position = transform.position;
            float y = -position.y + .8f, min = -5.5f, max = position.y - 5.8f;
            reflection.position = new Vector3(position.x, Mathf.Clamp(y, min, max), 0);
            var rotation = transform.rotation;
            reflection.rotation = new Quaternion(rotation.x, rotation.y, -rotation.z, rotation.w);
            reflectionSprite.color = new Color(1f, 1f, 1f, (y - min) / (max + 24f - min));
        }

        private void FixedUpdate()
        {
            /* movement */
            ship.AddForce(new Vector2(movement, 0) * Time.deltaTime * 50f * speed);
            ship.MoveRotation(Mathf.LerpAngle(ship.rotation, Mathf.Atan2(Mathf.Abs(movement) * 5f, -movement) * Mathf.Rad2Deg - 90f, Time.deltaTime * 50f * .01f));

            /* bounds */
            if (transform.position.x >= right.x) ship.MovePosition(left);
            else if (transform.position.x <= left.x) ship.MovePosition(right);
        }

        public void MoveLeft()
        {
            movement = -1f;
            Debug.Log("Left");
        }
        public void MoveRight()
        {
            movement = 1f;
            Debug.Log("Right");
        }
    }
}