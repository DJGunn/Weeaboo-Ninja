using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject enemyNinja;

    private GameObject thePlayer;
    private Text ninjaNumberText;
    private int ninjaNumber, leftToSpawn;
    private int numEnemies, maxNumEnemies;
    private float spawnTimeThreshold, timeSinceLastSpawn;

    // Use this for initialization
    void Start () {

        thePlayer = GameObject.Find("Player");
        leftToSpawn = ninjaNumber = (int)Random.Range(5.0f, 11.0f);
        ninjaNumberText = GameObject.Find("NinjaNumberText").GetComponent<Text>();
        numEnemies = 0;
        maxNumEnemies = 3;

        updateNinjaNumberText();

        spawnTimeThreshold = 0.0f;
        timeSinceLastSpawn = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {

        if (spawnTimeThreshold <= timeSinceLastSpawn && numEnemies < maxNumEnemies && leftToSpawn > 0)
        {

            Instantiate(enemyNinja, new Vector3(Random.Range(-5.5f, 5.5f), Random.Range(-4.0f, 4.0f), 0), Quaternion.identity);

            numEnemies++;
            leftToSpawn--;

            timeSinceLastSpawn = 0;
            spawnTimeThreshold = Random.Range(1.0f, 7.0f);
        }
        else timeSinceLastSpawn += Time.deltaTime;

        if (ninjaNumber <= 0)
        {
            thePlayer.gameObject.SetActive(false);
            thePlayer.gameObject.GetComponent<PlayerController>().winGame = true;
            thePlayer.gameObject.GetComponent<PlayerController>().setGameOver();
        }
    }

    public void ninjaKilled()
    {
        numEnemies--;
        ninjaNumber--;
        updateNinjaNumberText();
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    private void updateNinjaNumberText()
    {
        ninjaNumberText.text = "Enemies: " + ninjaNumber;
    }
}
