using System.Collections;
using Cubequad.Tentacles2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Color[] colorPool = { new Color(182, 222, 42), new Color(232, 140, 55), new Color(48, 222, 223), new Color(255, 109, 109) };
    public Sprite[] shapes = new Sprite[4];
    public int id;
    public GameObject blob;
    public GameObject gameLogics;
    public GameObject blobSprite;

    int currentSpriteIndex = 0;
    int currentColorIndex = 0;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction confirmAction;
    InputAction cancelAction;
    GameObject blobPreview;
    GameObject squidCustomizer;
    bool inputOnCooldown = false;
    bool isSelectingColor = false;
    bool isSelectingShape = true;
    bool isHoveringReady = false;
    bool playerReady = false;
    bool active = true;
    private IEnumerator buttonBCoroutine;

    // Use this for initialization
    void Start()
    {
        blobSprite = FindChild(blob, "SpriteBlob");
        squidCustomizer = FindChild(gameObject, "SquidCustomizer");
        blobPreview = FindChild(squidCustomizer, "BlobPreview");
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        confirmAction = playerInput.actions["Confirm"];
        cancelAction = playerInput.actions["Cancel"];

        confirmAction.started += ctx =>
        {
            if(this == null)
            {
                return;
            }
            if (!active)
            {
                transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                active = true;
                GameObject.Find("PlayerPressLabel" + id).transform.localScale = new Vector3(0, 0, 0);
                return;
            }
            if(isHoveringReady && !playerReady)
            {
                GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().ConfirmSound();
                playerReady = true;
                ActivateTentacles();
                gameLogics.GetComponent<GameLogics>().PlayerReady(id, colorPool[currentColorIndex], shapes[currentSpriteIndex]);
            } else if (isSelectingShape) {
                SelectColor();
                GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            } else if (isSelectingColor) {
                HoverReady();
                GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            }
        };

        cancelAction.started += ctx =>
        {
            if (this == null || isSelectingShape)
            {
                return;
            }
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            if (isHoveringReady && playerReady)
            {
                playerReady = false;
                StopTentacles();
            }
            else if (isHoveringReady)
            {
                SelectColor();
            }
            else if (isSelectingColor)
            {
                SelectShape();
            }
        };

        playerInput.actions["MainMenu"].performed += ctx =>
        {
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().CancelSound();
            if (this == null)
            {
                return;
            }
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        };

        playerInput.actions["MainMenu"].canceled += ctx =>
        {
            if (this == null)
            {
                return;
            }
            StopCoroutine(buttonBCoroutine);
            Material mat = FindChild(GameObject.Find("SelectionMenu"), "ABoutonRetour").GetComponent<SpriteRenderer>().material;
            mat.SetFloat("_Slider", 0);
        };

        playerInput.actions["MainMenu"].started += ctx =>
        {
            if (this == null)
            {
                return;
            }
            buttonBCoroutine = ButtonBSlideUp();
            StartCoroutine(buttonBCoroutine);
        };
    }

    IEnumerator ButtonBSlideUp()
    {
        float t;
        Material mat = FindChild(GameObject.Find("SelectionMenu"), "ABoutonRetour").GetComponent<SpriteRenderer>().material;
        t = 0.0f;
        while (t < 0.4f)
        {
            mat.SetFloat("_Slider",  Mathf.Lerp(0, 1, t * 2.5f));
            t += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_Slider", 1);
    }

    void Update()
    {
        if (inputOnCooldown || playerReady || isHoveringReady)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.x > 0.9f || moveDirectionVector.y < -0.9f)
        {
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            StartCoroutine(EnableInput(0.3f));
            inputOnCooldown = true;
            if(isSelectingShape)
            {
                currentSpriteIndex++;
                if (currentSpriteIndex >= shapes.Length)
                {
                    currentSpriteIndex = 0;
                }
                ApplyShape();
            }
            if (isSelectingColor)
            {
                currentColorIndex++;
                if (currentColorIndex >= colorPool.Length)
                {
                    currentColorIndex = 0;
                }
                ApplyColor();
            }
        }
        if (moveDirectionVector.x < -0.9f || moveDirectionVector.y > 0.9f)
        {
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            StartCoroutine(EnableInput(0.3f));
            inputOnCooldown = true;
            if (isSelectingShape)
            {
                currentSpriteIndex--;
                if (currentSpriteIndex < 0)
                {
                    currentSpriteIndex = shapes.Length - 1;
                }
                ApplyShape();
            }
            if (isSelectingColor)
            {
                currentColorIndex--;
                if (currentColorIndex < 0)
                {
                    currentColorIndex = colorPool.Length - 1;
                }
                ApplyColor();
            }
        }

    }

    void ActivateTentacles()
    {
        foreach (Transform child in FindChild(blobPreview, "Tentacles").transform)
        {
            child.gameObject.GetComponent<Tentacle>().Animation = Tentacle.Animations.wave;
        }
    }

    void StopTentacles()
    {
        foreach (Transform child in FindChild(blobPreview, "Tentacles").transform)
        {
            child.gameObject.GetComponent<Tentacle>().Animation = Tentacle.Animations.none;
        }
    }

    void ApplyColor()
    {
        FindChild(FindChild(gameObject, "SquidCustomizer"), "ColorPicker").GetComponent<SpriteRenderer>().color = colorPool[currentColorIndex];
        FindChild(FindChild(FindChild(gameObject, "SquidCustomizer"), "ColorPicker"), "Arrows").GetComponent<SpriteRenderer>().color = colorPool[currentColorIndex];
        FindChild(FindChild(FindChild(FindChild(gameObject, "SquidCustomizer"), "ColorPicker"), "Arrows"), "Arrows (3)").GetComponent<SpriteRenderer>().color = colorPool[currentColorIndex];
        blobPreview.GetComponent<SpriteRenderer>().color = colorPool[currentColorIndex];
        blobSprite.GetComponent<SpriteRenderer>().color = colorPool[currentColorIndex];
        
        foreach (Transform child in FindChild(blobPreview, "Tentacles").transform)
        {
            child.gameObject.GetComponent<Tentacle>().Color = colorPool[currentColorIndex];
        }
        foreach (Transform child in FindChild(blobSprite, "Tentacles").transform)
        {
            child.gameObject.GetComponent<Tentacle>().Color = colorPool[currentColorIndex];
        }
    }

    void ApplyShape()
    {
        blobPreview.GetComponent<SpriteRenderer>().sprite = shapes[currentSpriteIndex];
        blobSprite.GetComponent<SpriteRenderer>().sprite = shapes[currentSpriteIndex];
    }

    void SelectShape()
    {
        isSelectingShape = true;
        isSelectingColor = false;
        isHoveringReady = false;
        FindChild(FindChild(squidCustomizer, "SquidPicker"), "Arrows").SetActive(true);
        FindChild(FindChild(squidCustomizer, "ColorPicker"), "Arrows").SetActive(false);
        FindChild(FindChild(squidCustomizer, "Ready"), "Ready Active").SetActive(false);
    }

    void SelectColor()
    {
        isSelectingShape = false;
        isSelectingColor = true;
        isHoveringReady = false;
        FindChild(FindChild(squidCustomizer, "SquidPicker"), "Arrows").SetActive(false);
        FindChild(FindChild(squidCustomizer, "ColorPicker"), "Arrows").SetActive(true);
        FindChild(FindChild(squidCustomizer, "Ready"), "Ready Active").SetActive(false);
    }

    void HoverReady()
    {
        isSelectingShape = false;
        isSelectingColor = false;
        isHoveringReady = true;
        FindChild(FindChild(squidCustomizer, "SquidPicker"), "Arrows").SetActive(false);
        FindChild(FindChild(squidCustomizer, "ColorPicker"), "Arrows").SetActive(false);
        FindChild(FindChild(squidCustomizer, "Ready"), "Ready Active").SetActive(true);
    }

    IEnumerator EnableInput(float duration)
    {
        yield return new WaitForSeconds(duration);
        inputOnCooldown = false;
    }

    GameObject FindChild(GameObject parent, string name)
    {
        foreach (Transform t in parent.transform)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
