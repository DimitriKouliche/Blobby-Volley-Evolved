using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cubequad.Tentacles2D
{
    public class ButtonHighlight : MonoBehaviour
    {
        private Image image;
        private Color32 normal = new Color32(33, 43, 78, 127), highlighted = new Color32(33, 43, 78, 220);
        private bool isHighlighted, wasHighlighted;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (isHighlighted)
            {
                Highlight();
                isHighlighted = false;
                wasHighlighted = true;
            }
            else if (wasHighlighted)
            {
                Grayout();
                wasHighlighted = false;
            }
        }

        public void Highlight()
        {
            if (image != null)
                image.color = highlighted;
        }

        public void Grayout()
        {
            if (image != null)
                image.color = normal;
        }

        public void HighlightUpdate()
        {
            isHighlighted = true;
        }
    }
}