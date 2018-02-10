using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public int score;
    public float speed;
    public bool winGame;
    public bool iFrames; //If true, player invincible
    public float iFrameTimer; //How long we want the player to be invincible on damage
    public GameObject projectilePrefab;

    private Vector2 cursorInWorldPos, movement, myPos;
    private bool iFrameBlink, isGamePaused, isGameOver; //iFrame, game pause, and game over flags
    private Animator animator;
    private Text scoreText;
    private Text healthText;
    private Text finalScoreText;
    private GameObject backgroundLose, backgroundWin, sakuraBackground;
    private float timeSinceLastShot;
    private float shotTimeThreshold;
    

    public int health;

    // Use this for initialization
    void Start () {

        score = 0;
        health = 5;

        movement = new Vector2();
        myPos = new Vector2();

        //player does not start invincible, sorry :P
        iFrames = iFrameBlink = false; //default to not blinking and not invulnerable
        iFrameTimer = 2.0f; // default 1 sec invulnerability

        isGameOver = isGamePaused = false; //game is not over or paused by default

        //get our animator
        animator = GetComponent<Animator>();

        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        backgroundLose = GameObject.Find("BackgroundLose");
        backgroundWin = GameObject.Find("BackgroundWin");
        sakuraBackground = GameObject.Find("SakuraBackground");

        updateGameText();

        backgroundLose.SetActive(false);
        backgroundWin.SetActive(false);
        sakuraBackground.SetActive(false);

        timeSinceLastShot = 0.0f;
        shotTimeThreshold = 0.35f;

        winGame = false;
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "EnemyBullet")
        {
            health--;
            iFrames = true; //gain momentary invulnerability
            Destroy(coll.gameObject);
            animator.SetTrigger("ninjaHit");

            if (health <= 0)
            {
                updateGameText();
                animator.SetBool("ninjaDead", true);
                setGameOver();
            }
        }
    }

    //occurs every frame
    void Update()
    {
        if (!isGameOver && !isGamePaused) //only update the game logic if the game isn't over or isn't paused
        {
            if (!iFrames)
                score++;

            updateGameText();


            if (Input.GetButtonDown("Fire1") && timeSinceLastShot > shotTimeThreshold)
            {
                timeSinceLastShot = 0;

                cursorInWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 direction = cursorInWorldPos - myPos;
                direction.Normalize();
                GameObject projectile = (GameObject)Instantiate(projectilePrefab, myPos, Quaternion.identity);
                Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
                projectile.GetComponent<Rigidbody2D>().velocity = direction * speed;

                //set TTL for the shuriken
                Destroy(projectile, 3.0f);
            }
            else timeSinceLastShot += Time.deltaTime;

        } //code below this line will still run when the game is paused or over
    }

    void updateGameText()
    {
        scoreText.text = "Score: " + score;
        healthText.text = "Health: " + health;
    }

    //occurs based on set time, independent of frame
    void FixedUpdate()
    {
        if (!isGameOver && !isGamePaused) //only update the game logic if the game isn't over or isn't paused
        {
            //updateScoreText();
            //get player's inputs and move us
            movement.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
            movement.y = Input.GetAxisRaw("Vertical") * Time.deltaTime * speed;

            if (movement.x != 0 || movement.y != 0) animator.SetBool("ninjaWalk", true);
            else animator.SetBool("ninjaWalk", false);

            gameObject.GetComponent<Rigidbody2D>().velocity = movement * speed * 10;

            myPos = gameObject.transform.position;

            movement.x = 0f;
            movement.y = 0f;

            if (iFrames) //if we go invincible
            {
                if (iFrameTimer > 0)
                {
                    //reduce time
                    iFrameTimer -= Time.deltaTime;

                    if (!iFrameBlink) //if player is visible
                    {
                        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 255); //make the player transparent
                        iFrameBlink = !iFrameBlink; // toggle iframe blink
                    }
                    else //player is blinking
                    {
                        GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 255); //make the player apparent
                        iFrameBlink = !iFrameBlink; // toggle iframe blink
                    }
                }
                else //turn off iframes, reset timer, clear blink flag
                {
                    iFrames = false; //turn off iframes
                    iFrameTimer = 2.0f; //reset timer
                    if (iFrameBlink) GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 255); //Ensure the player is visible if they weren't
                    iFrameBlink = false; //ensure iframe isn't blinking anymore
                }
            }
        } //anything below this will still run when the game is paused or over


    }

    void toggleGamePause()
    {
        isGamePaused = !isGamePaused;
    }

    public void setGameOver()
    {
        isGameOver = true;
        //animator.SetBool("ninjaWalk", false);
        gameObject.GetComponent<Rigidbody2D>().velocity = movement * 0;

        if (!winGame) backgroundLose.SetActive(true);
        else
        {
            backgroundWin.SetActive(true);
            sakuraBackground.SetActive(true);
        }

        finalScoreText = GameObject.Find("FinalScoreText").GetComponent<Text>();
        finalScoreText.text = "Final Score: " + score;
    }
}
