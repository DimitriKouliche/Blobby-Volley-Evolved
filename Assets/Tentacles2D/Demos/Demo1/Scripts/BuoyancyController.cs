using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class BuoyancyController : MonoBehaviour
    {
        public float amplitude, frequency;
        private float level;
        private BuoyancyEffector2D buoyancyEffector;

        private void Awake()
        {
            buoyancyEffector = GetComponent<BuoyancyEffector2D>();
            level = buoyancyEffector.surfaceLevel;
        }

        private void FixedUpdate()
        {
            if (buoyancyEffector != null)
                buoyancyEffector.surfaceLevel = level + amplitude * (Mathf.Sin(2 * Mathf.PI * frequency * Time.time) - Mathf.Sin(2 * Mathf.PI * frequency * (Time.time - Time.deltaTime)));
        }
    }
}