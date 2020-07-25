using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubequad.Tentacles2D
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField] private float delay = 3f;
        private AreaEffector2D forceField;
        private Transform arrows;
        private float initialDelay;
        private bool moveRight;

        private void Awake()
        {
            forceField = GetComponent<AreaEffector2D>();
            arrows = transform.GetChild(0);
            initialDelay = delay;
        }

        private void Update()
        {
            if (delay > 0)
                delay -= Time.deltaTime;
            else
            {
                forceField.forceAngle = moveRight ? 0 : 180f;

                var arrowsScale = arrows.localScale;
                arrowsScale.x = -arrowsScale.x;
                arrows.localScale = arrowsScale;

                delay = initialDelay;
                moveRight = !moveRight;
            }
        }
    }
}