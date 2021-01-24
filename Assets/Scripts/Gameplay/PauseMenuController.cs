using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject[] arrows;
    public GameObject keyboardControls;
    public GameObject gamepadControls;
    public GameObject soundMenu;
    public GameObject gameLogics;
    int menuId = 0;
    InputAction moveAction;
    bool inputOnCooldown = false;
    bool justWokeUp = true;
    bool isOnControl = false;
    bool isOnSound = false;
    bool isOnGamePad = true;
    bool isOnKeyboard = false;
    bool isOnMusic = false;
    bool isOnSFX = false;
    float musicVolume;
    float sfxVolume;
    Vector3 SFXJaugeOrigin = new Vector3(0.19f, -6.94f, 0);
    Vector3 SFXJaugeDestination = new Vector3(0.22f, -15.87f, 0);
    Vector3 musicJaugeOrigin = new Vector3(0.58f, -7f, 0);
    Vector3 musicJaugeDestination = new Vector3(0.61f, -16.61f, 0);


    // Use this for initialization
    void OnEnable()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        FindChild(FindChild(soundMenu, "ABoutonSFX"), "SFXJaugeA").transform.localPosition = Vector3.Lerp(SFXJaugeOrigin, SFXJaugeDestination, 1 - sfxVolume / 100f);
        FindChild(FindChild(soundMenu, "ABoutonMusic"), "MusicJaugeA").transform.localPosition = Vector3.Lerp(musicJaugeOrigin, musicJaugeDestination, 1 - musicVolume / 100f);
        justWokeUp = true;
        moveAction = playerInput.actions["Move"];
        inputOnCooldown = false;

        playerInput.actions["Charge Jump"].started += ctx =>
        {
            if (this == null || ! gameObject.activeSelf)
            {
                return;
            }
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
            switch (menuId)
            {
                case 0:
                    gameLogics.GetComponent<GameLogics>().isPaused = false;
                    Time.timeScale = 1.2f;
                    gameObject.SetActive(false);
                    break;
                case 1:
                    gameLogics.GetComponent<GameLogics>().isPaused = false;
                    gameObject.SetActive(false);
                    gameLogics.GetComponent<GameLogics>().RestartGame();
                    Time.timeScale = 1.2f;
                    break;
                case 4:
                    gameLogics.GetComponent<GameLogics>().isPaused = false;
                    Time.timeScale = 1.2f;
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                    break;
            }
        };

        playerInput.actions["Start"].started += ctx =>
        {
            if (playerInput == null || this == null || !gameObject.activeSelf)
            {
                return;
            }
            playerInput = null;
            if(justWokeUp)
            {
                GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().AppearSound();
                justWokeUp = false;
                return;
            }
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().CancelSound();
            StartCoroutine(EnableInput(0.2f));
            gameObject.SetActive(false);
            Time.timeScale = 1;
        };

        // Dashing
        playerInput.actions["Dash"].started += ctx =>
        {
            if (this == null || !gameObject.activeSelf)
            {
                return;
            }
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().CancelSound();
            Time.timeScale = 1;
            gameObject.SetActive(false);
        };
    }

    void Update()
    {
        if (this == null || !gameObject.activeSelf || moveAction == null || inputOnCooldown)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.y > 0.9f)
        {
            MoveToPrevious();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.2f));
        }
        if (moveDirectionVector.y < -0.9f)
        {
            MoveToNext();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.2f));
        }
        if (moveDirectionVector.x > 0.9f)
        {
            HandleXMovement(true);
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
        }
        if (moveDirectionVector.x < -0.9f)
        {
            HandleXMovement(false);
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
        }
    }

    void MoveToNext()
    {
        GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
        if (isOnSFX)
        {
            if (sfxVolume <= 0)
            {
                return;
            }
            sfxVolume -= 5;
            FindChild(FindChild(soundMenu, "ABoutonSFX"), "SFXJaugeA").transform.localPosition = Vector3.Lerp(SFXJaugeOrigin, SFXJaugeDestination, 1 - sfxVolume / 100f);
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
            return;
        }
        if (isOnMusic)
        {
            if (musicVolume <= 0)
            {
                return;
            }
            musicVolume -= 5;
            FindChild(FindChild(soundMenu, "ABoutonMusic"), "MusicJaugeA").transform.localPosition = Vector3.Lerp(musicJaugeOrigin, musicJaugeDestination, 1 - musicVolume / 100f);
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().UpdateVolume();
            return;
        }
        if (menuId >= 4)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId++;
        isOnSound = menuId == 3;
        soundMenu.SetActive(menuId == 3);
        isOnControl = menuId == 2;
        transform.GetChild(menuId).GetChild(0).gameObject.SetActive(true);
    }

    void MoveToPrevious()
    {
        GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
        if (isOnSFX)
        {
            if (sfxVolume >= 100)
            {
                return;
            }
            sfxVolume += 5;
            FindChild(FindChild(soundMenu, "ABoutonSFX"), "SFXJaugeA").transform.localPosition = Vector3.Lerp(SFXJaugeOrigin, SFXJaugeDestination, 1 - sfxVolume / 100f);
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
            return;
        }
        if (isOnMusic)
        {
            if (musicVolume >= 100)
            {
                return;
            }
            musicVolume += 5;
            FindChild(FindChild(soundMenu, "ABoutonMusic"), "MusicJaugeA").transform.localPosition = Vector3.Lerp(musicJaugeOrigin, musicJaugeDestination, 1 - musicVolume / 100f);
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            GameObject.Find("Music(Clone)").GetComponent<MusicMixer>().UpdateVolume();
            return;
        }
        if (menuId <= 0)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId--;
        isOnSound = menuId == 3;
        soundMenu.SetActive(menuId == 3);
        isOnControl = menuId == 2;
        transform.GetChild(menuId).GetChild(0).gameObject.SetActive(true);
    }

    IEnumerator EnableInput(float duration)
    {
        yield return StartCoroutine(WaitForRealSeconds(duration));
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
    IEnumerator WaitForRealSeconds(float seconds)
    {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }
    }
    void HandleXMovement(bool isRight)
    {
        GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
        if (isOnControl)
        {
            if (isOnGamePad && isRight)
            {
                isOnGamePad = false;
                isOnKeyboard = true;
            }
            else if (isOnKeyboard && !isRight)
            {
                isOnGamePad = true;
                isOnKeyboard = false;
            }
            keyboardControls.SetActive(isOnKeyboard);
            gamepadControls.SetActive(isOnGamePad);
        }
        if (isOnSound)
        {
            if (!isOnMusic && !isOnSFX && isRight)
            {
                isOnMusic = true;
            }
            else if (isOnMusic && isRight)
            {
                isOnMusic = false;
                isOnSFX = true;
            }
            else if (isOnSFX && !isRight)
            {
                isOnMusic = true;
                isOnSFX = false;
            }
            else if (isOnMusic && !isRight)
            {
                isOnMusic = false;
                isOnSFX = false;
            }
            FindChild(FindChild(soundMenu, "ABoutonMusic"), "ParticleSelection").SetActive(isOnMusic);
            FindChild(FindChild(soundMenu, "ABoutonSFX"), "ParticleSelection").SetActive(isOnSFX);
        }
    }

}
