using System;
using System.Collections;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    Rigidbody playerRb;
    Animator playerAnimator;
    GameManager gameManager;
    [SerializeField] GameObject swordHand;
    // Player Values
    [SerializeField] int playerNum;
    readonly float speed = 2500;
    float horizontalInput, forwardInput;
    readonly float zLowBound = -6, zHighBound = 12, xBound = 15;
    bool hasCollided;
    // Player Number Property
    public int PlayerNumber {get{return playerNum;}}
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        PreventOffScreen();
        if (playerAnimator.GetInteger("WeaponType_int") == 4)
        {
            HoldSword(GameObject.FindGameObjectWithTag("Weapon"));
            SwingSword();
        }
    }
    // Move the player using either the arrow keys or the joystick
    void Move() {
        // Define input of horizontal and vertical axis on either player
        switch (playerNum)
        {
            case 1:
                horizontalInput = gameManager.GameActive ? Input.GetAxis("Horizontal") : 0;
                forwardInput = gameManager.GameActive ? Input.GetAxis("Vertical") * gameManager.InvertedStatus : 0;
                break;
            case 2:
                horizontalInput = gameManager.GameActive ? Input.GetAxis("Horizontal2") : 0;
                forwardInput = gameManager.GameActive ? Input.GetAxis("Vertical2") * gameManager.InvertedStatus : 0;
                break;
        }
        Turn();
        // Set Player Speed Animation
        playerAnimator.SetFloat("Speed_f", Math.Abs(horizontalInput) + Math.Abs(forwardInput));
        // Move the player by adding forces on x- and z-axis while the game is active
        if (gameManager.GameActive)
        {
            playerRb.AddForce(forwardInput * speed * Time.deltaTime * Vector3.forward, ForceMode.Impulse);
            playerRb.AddForce(horizontalInput * speed * gameManager.InvertedStatus * Time.deltaTime * Vector3.right, ForceMode.Impulse);
        }
    }
    /* Set movement animation based on speed and rotation based on where
       player is facing */
    void Turn()
    {
        float angle;
        if (horizontalInput == 0 && forwardInput < 0)
        {
            angle = 180;
        }
        else if (forwardInput == 0)
        {
            angle = (float)(Math.Asin(horizontalInput) * (180 / Math.PI));
        }
        else if (forwardInput < 0)
        {
            angle = (horizontalInput < 0) ? -135 : 135;
        }
        else
        {
            angle = (float)(Math.Atan(horizontalInput / forwardInput) * (180 / Math.PI));
        }
        angle *= gameManager.InvertedStatus;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    // Prevents the Player from the leaving the screen
    void PreventOffScreen() {
        // Z-Axis
        if (transform.position.z < zLowBound) {
            transform.position = new Vector3(
                transform.position.x, transform.position.y, zLowBound
            );
        }
        if (transform.position.z > zHighBound) {
            transform.position = new Vector3(
                transform.position.x, transform.position.y, zHighBound
            );
        }
        //X-Axis
        if (transform.position.x < -xBound) {
            transform.position = new Vector3(
                -xBound, transform.position.y, transform.position.z
            );
        }
        if (transform.position.x > xBound) {
            transform.position = new Vector3(
                xBound, transform.position.y, transform.position.z
            );
        }
    }
    // After losing a life, gain a 3-second invulnerability 
    IEnumerator Invulnerability()
    {
        yield return new WaitForSecondsRealtime(3);
        hasCollided = false;

    }
    // After swinging a sword, gain a 2-second cooldown
    IEnumerator SwingCoolDown()
    {
        yield return new WaitForSecondsRealtime(2);
        playerAnimator.SetBool("Swing_b", false);
    }
    // OnCollisionEnter is called when the player collides with an enemy
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle") && !hasCollided) {
            hasCollided = true;
            StartCoroutine(Invulnerability());
            switch (PlayerNumber)
            {
                case 1:
                    gameManager.P1Lives--;
                    break;
                case 2:
                    gameManager.P2Lives--;
                    break;
            }
            gameManager.UpdateLives(PlayerNumber);
            if (gameManager.P1Lives == 0 || gameManager.P2Lives == 0)
            {
                gameManager.P1Score += gameManager.P1Lives;
                gameManager.P2Score += gameManager.P2Lives;
                Debug.Log($"1 {gameManager.P1Lives}, 2 {gameManager.P2Lives}");
                playerAnimator.SetBool("Death_b", true);
                playerAnimator.SetInteger("DeathType_int", 2);
                gameManager.EndGame();
            }

        }
    }
    // Have the player hold a sword (for animation purposes)
    public void HoldSword(GameObject sword)
    {
        Quaternion holdRotation = 
            Quaternion.Euler(
                swordHand.transform.rotation.eulerAngles.x - 180, 
                swordHand.transform.rotation.eulerAngles.y + 225, 
                swordHand.transform.rotation.eulerAngles.z + 225
            );
        sword.transform.SetPositionAndRotation(
            swordHand.transform.position, holdRotation
        );
    }
    // Have the player swing a sword
    void SwingSword()
    {
        float swingInput = (playerNum == 1) ? Input.GetAxis("Swing") : Input.GetAxis("Swing2");
        if (swingInput > 0 && !playerAnimator.GetBool("Swing_b"))
        {
            playerAnimator.SetBool("Swing_b", true);
            StartCoroutine(SwingCoolDown());
        }
    }
    // OnTriggerEnter is called when the player collides with a consumable
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Consumable")) {
            gameManager.CollectFlag(PlayerNumber);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Weapon"))
        {
            playerAnimator.SetInteger("WeaponType_int", 4);
        }
    }
}
