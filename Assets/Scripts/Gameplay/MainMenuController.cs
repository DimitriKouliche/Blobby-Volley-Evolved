using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] arrows;
    public GameObject soundManager;
    public GameObject credits;
    public GameObject duelSplash;
    public GameObject partySplash;
    public GameObject duelTitle;
    public GameObject partyTitle;
    public GameObject soloTitle;
    public GameObject keyboardControls;
    public GameObject gamepadControls;
    public GameObject soundMenu;
    public GameObject UISound;
    public GameObject musicMixerPrefab;
    public string[] scenes;
    int menuId = 0;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction confirmAction;
    bool inputOnCooldown = false;
    bool splashScreen = false;
    bool isOnControl = false;
    bool isOnSound = false;
    bool isOnGamePad = true;
    bool isOnKeyboard = false;
    bool isOnMusic = false;
    bool isOnSFX = false;
    float musicVolume;
    float sfxVolume;
    GameObject musicMixer;
    Vector3 SFXJaugeOrigin = new Vector3(0.19f, -6.94f, 0);
    Vector3 SFXJaugeDestination = new Vector3(0.22f, -15.87f, 0);
    Vector3 musicJaugeOrigin = new Vector3(0.58f, -7f, 0);
    Vector3 musicJaugeDestination = new Vector3(0.61f, -16.61f, 0);

    // Use this for initialization
    void Start()
    {
        GameObject uiSound = GameObject.Find("UISound(Clone)");
        if(uiSound == null)
        {
            uiSound = GameObject.Instantiate(UISound);
        }
        musicMixer = GameObject.Find("Music(Clone)");
        if(musicMixer == null)
        {
            musicMixer = GameObject.Instantiate(musicMixerPrefab);
        }
        musicMixer.GetComponent<MusicMixer>().MenuMusic();
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 100f);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 100f);
        FindChild(FindChild(soundMenu, "ABoutonSFX"), "SFXJaugeA").transform.localPosition = Vector3.Lerp(SFXJaugeOrigin, SFXJaugeDestination, 1 - sfxVolume / 100f);
        FindChild(FindChild(soundMenu, "ABoutonMusic"), "MusicJaugeA").transform.localPosition = Vector3.Lerp(musicJaugeOrigin, musicJaugeDestination, 1 - musicVolume / 100f);
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        confirmAction = playerInput.actions["Confirm"];

        confirmAction.started += ctx =>
        {
            if (this == null || inputOnCooldown)
            {
                return;
            }
            GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().ConfirmSound();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
            if(FindChild(gameObject, "StartScreen").activeSelf)
            {
                FindChild(gameObject, "StartScreen").SetActive(false);
                FindChild(gameObject, "MenuContent").SetActive(true);
                splashScreen = false;
                return;
            }
            if(splashScreen)
            {
                return;
            }
            switch (menuId)
            {
                case 0:
                    PlayerPrefs.SetInt("numberPlayers", 4);
                    StartCoroutine(ConfirmAnimation("SplashScreen"));
                    break;
                case 1:
                    PlayerPrefs.SetInt("numberPlayers", 2);
                    StartCoroutine(ConfirmAnimation("SplashScreen"));
                    break;
                case 2:
                    PlayerPrefs.SetInt("numberPlayers", 1);
                    StartCoroutine(ConfirmAnimation("SplashScreen"));
                    break;
                case 3:
                    Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=2381185635");
                    break;
                case 7:
                    Application.Quit();
                    break;
            }
        };
    }

    IEnumerator ConfirmAnimation(string scene)
    {
        float t = 0.0f;
        var objects = GameObject.FindGameObjectsWithTag("MenuText");
        var particles = GameObject.FindGameObjectsWithTag("MenuParticle");
        while (t < 0.1f)
        {
            float alpha = Mathf.Lerp(1f, 0f, t*10);
            foreach (var obj in objects)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, alpha);
            }
            foreach (var particle in particles)
            {
                particle.SetActive(false);
            }
            t += Time.deltaTime;
            yield return null;
        }
        foreach (var obj in objects)
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(obj.GetComponent<SpriteRenderer>().color.r, obj.GetComponent<SpriteRenderer>().color.g, obj.GetComponent<SpriteRenderer>().color.b, 0);
        }
        t = 0;
        while (t < 0.2f)
        {
            float x = Mathf.Lerp(-3.2f, -8.8f, t * 5);
            FindChild(gameObject, "Black").transform.localPosition = new Vector3(x, FindChild(gameObject, "Black").transform.localPosition.y, FindChild(gameObject, "Black").transform.localPosition.z);
            t += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    void Update()
    {
        if (inputOnCooldown || splashScreen)
        {
            return;
        }
        // Moving
        var moveDirectionVector = moveAction.ReadValue<Vector2>();
        if (moveDirectionVector.y > 0.9f)
        {
            MoveToPrevious();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
        }
        if (moveDirectionVector.y < -0.9f)
        {
            MoveToNext();
            inputOnCooldown = true;
            StartCoroutine(EnableInput(0.15f));
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

    void HandleXMovement(bool isRight)
    {
        GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
        if(isOnControl)
        {
            if(isOnGamePad && isRight)
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
        if(isOnSound)
        {
            if(!isOnMusic && !isOnSFX && isRight)
            {
                isOnMusic = true;
            }
            else if(isOnMusic && isRight)
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


    void DisplayRightPanel()
    {
        soundManager.SetActive(false);
        credits.SetActive(false);
        partyTitle.SetActive(false);
        soloTitle.SetActive(false);
        duelTitle.SetActive(false);
        isOnControl = false;
        isOnSound = false;
        if (menuId == 0)
        {
            partySplash.SetActive(true);
            partyTitle.SetActive(true);
            duelSplash.SetActive(false);
        }
        if (menuId == 1)
        {
            duelSplash.SetActive(true);
            duelTitle.SetActive(true);
            partySplash.SetActive(false);
        }
        if (menuId == 2)
        {
            soloTitle.SetActive(true);
        }
        if (menuId == 4)
        {
            isOnControl = true;
        }
        if (menuId == 5)
        {
            isOnSound = true;
            soundManager.SetActive(true);
        }
        if (menuId == 6)
        {
            credits.SetActive(true);
        }
    }

    void MoveToNext()
    {
        GameObject.Find("UISound(Clone)").GetComponent<MenuSound>().SelectSound();
        if (isOnSFX)
        {
            if(sfxVolume <= 0)
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
        if (menuId > 6)
        {
            return;
        }
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(false);
        }
        menuId++;
        DisplayRightPanel();
        transform.GetChild(0).GetChild(menuId).GetChild(0).gameObject.SetActive(true);
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
        DisplayRightPanel();
        transform.GetChild(0).GetChild(menuId).GetChild(0).gameObject.SetActive(true);
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
