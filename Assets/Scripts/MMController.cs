using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMController : MonoBehaviour
{

    private GameObject backgroundMenu, backgroundInstructions;

    // Use this for initialization
    void Start()
    {
        backgroundMenu = GameObject.Find("BackgroundMenu");
        backgroundInstructions = GameObject.Find("BackgroundInstructions");

        //backgroundMenu.SetActive(false);
        backgroundInstructions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startGame()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public void showStory()
    {
        backgroundInstructions.SetActive(true);
        backgroundMenu.SetActive(false);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
