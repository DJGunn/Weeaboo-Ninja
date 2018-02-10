using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public float speed;
    public float destroyTimer;
    public GameObject projectilePrefab;

    private Vector2 movement, myPos;
    private bool isGamePaused, isGameOver, isDead; //iFrame, game pause, and game over flags
    private Animator animator;
    private GameObject thePlayer;
    private float timeSinceLastShot;
    private float shotTimeThreshold;

    public int health;

    // Use this for initialization
    void Start () {

        health = 1;

        movement = new Vector2();
        movement.x = Random.Range(-5.0f, 5.0f) * 1.5f;
        movement.y = Random.Range(-5.0f, 5.0f) * 1.5f;

        myPos = new Vector2();

        destroyTimer = 5.0f; // default 1 sec invulnerability

        isGameOver = isGamePaused = isDead = false; //game is not over or paused by default, and enemy is alive

        //get our animator
        animator = GetComponent<Animator>();
        animator.SetBool("ninjaWalk", true); //set player to walking

        thePlayer = GameObject.Find("Player");

        timeSinceLastShot = 0.0f;
        shotTimeThreshold = Random.Range(0.5f, 1.5f);
    }
    
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "PlayerBullet" && !isDead)
        {
            health--;
            Destroy(coll.gameObject);
            animator.SetTrigger("ninjaHit");

            if (health <= 0)
            {
                animator.SetBool("ninjaWalk", false);
                animator.SetBool("ninjaDead", true);
                gameObject.GetComponent<Rigidbody2D>().velocity = movement * 0.0f;

                //add score
                thePlayer.gameObject.GetComponent<PlayerController>().score += 5000;
                //enemy no longer functions
                isDead = true;

                //report ninja death
                GameObject.Find("GameController").GetComponent<GameController>().ninjaKilled();

                //destroy self in x seconds
                Destroy(gameObject, destroyTimer);
            }
        }
        if ((coll.gameObject.tag == "LevelBorder" || coll.gameObject.tag == "CornerProtect") && !isDead)
        {
            float dieRoll = Random.Range(-1.0f, 1.0f);
            if ( dieRoll >= 0.0f)
            {
                movement.x = Random.Range(-5.0f, 5.0f);
                movement.y = Random.Range(-5.0f, 5.0f);
                Invoke("RandomizeMovement", 2);
            }
            else
            {
                movement.x *= -1;
                movement.y *= -1;
                Invoke("RandomizeMovement", 2);
            }
        }
    }

    void RandomizeMovement()
    {
        if (isDead) return;

        movement.x = Random.Range(-5.0f, 5.0f);
        movement.y = Random.Range(-5.0f, 5.0f);
    }

    //occurs every frame
    void Update()
    {
        if (!isGameOver && !isGamePaused && !isDead) //only update the game logic if the game isn't over or isn't paused
        {
            if (timeSinceLastShot > shotTimeThreshold)
            {
                timeSinceLastShot = Random.Range(-1.5f, 1.0f); ;

                Vector2 direction = (Vector2)(thePlayer.transform.position) - myPos;
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


    //occurs based on set time, independent of frame
    void FixedUpdate()
    {
        //kill ninjas if player wins
        if (thePlayer.gameObject.GetComponent<PlayerController>().winGame) destroyNinjas();

        if (!isGameOver && !isGamePaused && !isDead) //only update the game logic if the game isn't over or isn't paused
        {

            gameObject.GetComponent<Rigidbody2D>().velocity = movement;

            myPos = gameObject.transform.position;
        } //anything below this will still run when the game is paused or over
    }

    //this can be called when ninjas need to be instantly destroyed
    public void destroyNinjas()
    {
        Destroy(gameObject);
    }
}
