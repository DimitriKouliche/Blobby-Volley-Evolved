using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class LeftRightPlatform : MonoBehaviour
    {
        [SerializeField] private float speed, offset;

        private Rigidbody2D platform;
        private Vector2 initialPosition, targetPosition;
        private bool movingRight = true;

        private void Awake()
        {
            platform = GetComponent<Rigidbody2D>();
            initialPosition = platform.position;
            targetPosition = new Vector2(initialPosition.x + offset, initialPosition.y);
        }

        private void FixedUpdate()
        {
            if (movingRight)
            {
                platform.AddForceAtPosition(Vector2.right * speed, targetPosition);
                //Debug.DrawLine(transform.position, targetPosition, Color.red);

                if (platform.position.x > targetPosition.x)
                    movingRight = false;
            }
            else
            {
                platform.AddForceAtPosition(Vector2.left * speed, targetPosition);
                //Debug.DrawLine(transform.position, targetPosition, Color.blue);

                if (platform.position.x < initialPosition.x)
                    movingRight = true;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = UnityEditor.Handles.color = Color.gray;
                var targetPosition = transform.position;
                targetPosition.x += offset;
                Gizmos.DrawLine(transform.position, targetPosition);
                UnityEditor.Handles.DrawWireDisc(targetPosition, Vector3.forward, 1f);
            }
        }
#endif
    }
}