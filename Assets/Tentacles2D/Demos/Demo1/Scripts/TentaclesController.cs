using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cubequad.Tentacles2D;

public class TentaclesController : MonoBehaviour
{
    private Tentacle[] tentacles;
    [SerializeField] private Rigidbody2D target;
    [SerializeField] private ButtonHighlight button;
    [SerializeField] private Text buttonText;
    [SerializeField] private SpriteRenderer shadow;
    public static bool isActive;
    private Color bgBright = new Color(0, 1f, .9294118f), bgDark = new Color(.3257956f, .8800711f, .952f), shadowBright = new Color(0, .3174366f, .7254902f, .7254902f), shadowDark = new Color(0, .3030323f, .671f, .8f);

    private void Awake()
    {
        tentacles = FindObjectsOfType<Tentacle>();
    }
    
    private void Update()
    {
        /* controls */
        if (Input.touchCount > 0) // touch
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && Camera.main.ScreenToWorldPoint(touch.position).y < -7f)
            {
                button.Highlight();
                SwitchTentacles();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                button.Grayout();
            }
        }
        else if (Input.GetMouseButtonDown(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).y < -7f) // mouse
        {
            button.Highlight();
            SwitchTentacles();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            button.Grayout();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // keyboard
        {
            button.Highlight();
            SwitchTentacles();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            button.Grayout();
        }
    }
    
    private void SwitchTentacles()
    {
        /* activate tentacles by setting/removing target */
        if (!isActive)
        {
            for (int i = 0; i < tentacles.Length; i++)
                tentacles[i].TargetRigidbody = target;
            //tentacles[i].TargetTransform = transform;
            isActive = true;

            /* decorations */
            buttonText.text = "Deactivate tentacles";
            AnimateColors(shadowDark, bgDark);
        }
        else
        {
            for (int i = 0; i < tentacles.Length; i++)
                tentacles[i].TargetRigidbody = null;
            isActive = false;

            /* decorations */
            buttonText.text = "Activate tentacles";
            AnimateColors(shadowBright, bgBright);
        }
    }

    /* decoration */
    private Coroutine animateColors;
    private void AnimateColors(Color tint, Color bg)
    {
        if (animateColors != null) StopCoroutine(animateColors);
        animateColors = StartCoroutine(AnimateColorsCor(tint, bg));
    }
    private IEnumerator AnimateColorsCor(Color tint, Color bg)
    {
        float currentTime = 0, time = 10f;
        var camera = Camera.main;
        while (currentTime <= time)
        {
            currentTime += Time.deltaTime; var t = currentTime / time;
            shadow.color = Color.Lerp(shadow.color, tint, t);
            camera.backgroundColor = Color.Lerp(camera.backgroundColor, bg, t);
            yield return null;
        }
    }
}
