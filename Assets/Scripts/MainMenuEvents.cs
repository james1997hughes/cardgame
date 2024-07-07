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
        SceneManager.LoadScene("Game");
    }
    public void loadbetaGame()
    {
        SceneManager.LoadScene("betaGame");
    }
    public void loadCardInspect()
    {
        SceneManager.LoadScene("CardInspect");
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
