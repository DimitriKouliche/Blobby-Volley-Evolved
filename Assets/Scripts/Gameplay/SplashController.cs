using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    public GameObject rules1v1;
    public GameObject rules2v2;
    InputAction confirmAction;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("numberPlayers") == 4)
        {
            rules2v2.SetActive(true);
            rules1v1.SetActive(false);
        }
        confirmAction = GetComponent<PlayerInput>().actions["Confirm"];

        confirmAction.started += ctx =>
        {
            if (this == null)
            {
                return;
            }
            if (PlayerPrefs.GetInt("numberPlayers") == 4)
            {
                StartCoroutine(ConfirmAnimation("Local4"));
            } else
            {
                StartCoroutine(ConfirmAnimation("Local2"));
            }
        };
    }
    IEnumerator ConfirmAnimation(string scene)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
