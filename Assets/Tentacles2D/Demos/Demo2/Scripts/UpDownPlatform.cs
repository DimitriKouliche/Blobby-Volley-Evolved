using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class UpDownPlatform : MonoBehaviour
    {
        [SerializeField] private float speed, offset;

        private Rigidbody2D platform;
        private Vector2 initialPosition, targetPosition;
        private bool movingDown;

        private void Awake()
        {
            platform = GetComponent<Rigidbody2D>();
            initialPosition = platform.position;
            targetPosition = new Vector2(initialPosition.x, initialPosition.y - offset);
        }

        private void FixedUpdate()
        {
            if (movingDown)
            {
                platform.AddForceAtPosition(Vector2.down * speed, targetPosition);
                if (platform.position.y < targetPosition.y)
                    movingDown = false;
            }
            else
            {
                platform.AddForceAtPosition(Vector2.up * speed, targetPosition);
                if (platform.position.y > initialPosition.y)
                    movingDown = true;
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.color = UnityEditor.Handles.color = Color.gray;
                var targetPosition = transform.position;
                targetPosition.y -= offset;
                Gizmos.DrawLine(transform.position, targetPosition);
                UnityEditor.Handles.DrawWireDisc(targetPosition, Vector3.forward, 1f);
            }
        }
#endif
    }
}