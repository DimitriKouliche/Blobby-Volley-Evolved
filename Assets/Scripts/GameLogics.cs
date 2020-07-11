using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogics : MonoBehaviour
{
    public GameObject uiBeginning;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1;
            uiBeginning.SetActive(false);
        }
    }
}
