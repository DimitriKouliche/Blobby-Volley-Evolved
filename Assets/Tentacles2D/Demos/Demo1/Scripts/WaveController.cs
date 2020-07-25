using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class WaveController : MonoBehaviour
    {
        [SerializeField] private float speed, amplitude, frequency/*, offset*/;
        private SpriteRenderer spriteRenderer;
        private float spriteWidth;
        private Vector2 initPosition;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteWidth = spriteRenderer.bounds.size.x / spriteRenderer.sprite.pixelsPerUnit / transform.localScale.x;
            initPosition = transform.localPosition;
        }

        private void Update()
        {
            transform.localPosition += amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * transform.up + new Vector3(speed, 0, 0);
            if (transform.localPosition.x >= spriteWidth) transform.localPosition = new Vector3(initPosition.x, transform.localPosition.y);
        }
    }
}