using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    bool duel = false;
    bool foursome = false;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction confirmAction;
    InputAction cancelAction;
    bool inputOnCooldown = false;

    // Use this for initialization
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        confirmAction = playerInput.actions["Confirm"];
        cancelAction = playerInput.actions["Cancel"];

        confirmAction.started += ctx =>
        {
        };

        cancelAction.started += ctx =>
        {
        };
    }

    void Update()
    {
        if (inputOnCooldown)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.y > 0.9f)
        {
            StartCoroutine(EnableInput(0.3f));
        }
        if (moveDirectionVector.y < -0.9f)
        {
            StartCoroutine(EnableInput(0.3f));
        }

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
