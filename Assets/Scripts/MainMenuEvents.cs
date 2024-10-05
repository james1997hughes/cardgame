using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadGame()
    {
        SceneManager.LoadScene("Game",LoadSceneMode.Single);
    }
    public void loadbetaGame()
    {
        SceneManager.LoadScene("betaGame",LoadSceneMode.Single);
    }
    public void loadCardInspect()
    {
        SceneManager.LoadScene("CardInspect",LoadSceneMode.Single);
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
    }
}
