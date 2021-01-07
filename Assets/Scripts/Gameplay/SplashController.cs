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
    InputAction cancelAction;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("numberPlayers") == 4)
        {
            rules2v2.SetActive(true);
            rules1v1.SetActive(false);
        }
        confirmAction = GetComponent<PlayerInput>().actions["Confirm"];
        cancelAction = GetComponent<PlayerInput>().actions["Cancel"];

        confirmAction.started += ctx =>
        {
            if (this == null)
            {
                return;
            }
            StartCoroutine(ConfirmAnimation("LocalAI"));
        };
        cancelAction.started += ctx =>
        {
            if (this == null)
            {
                return;
            }
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        };
    }
    IEnumerator ConfirmAnimation(string scene)
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
