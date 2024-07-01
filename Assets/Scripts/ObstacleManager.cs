using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // Components
    Rigidbody obstacleRb;
    Animator obstacleAnimator;
    GameManager gameManager;
    AudioSource obstacleAudio;
    // Obstacle Values
    float zSpeed = 0, xSpeed = 0, moveSpeed = 200;
    readonly float zLowBound = -7, zHighBound = 13, xBound = 13;
    float yOffSet;
    // Start is called before the first frame update
    void Start()
    {
        obstacleRb = GetComponent<Rigidbody>();
        obstacleAudio = GetComponent<AudioSource>();
        obstacleAudio.volume = GameObject.Find("Main Camera").GetComponent<AudioSource>().volume;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        RampSpeed();
        SetAxisSpeed();
        obstacleAnimator = GetComponent<Animator>();

        if (obstacleRb.mass == 70)
        {
            yOffSet = 0.75f;
        }
        else
        {
            yOffSet = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (obstacleAudio.volume != gameManager.SFXVolume)
        {
            SetObstacleAudio();
        }
        /* Only apply to every obstacle but the boulder while the game is 
           active */
        if (obstacleRb.mass != 70)
        {
            if (gameManager.GameActive)
            {
                obstacleAnimator.SetFloat("Speed_f", 1);
            }
            else if (!gameManager.GameActive)
            {
                obstacleAnimator.SetFloat("Speed_f", 0);
            }
        }
        // Only move when the game is active
        if (gameManager.GameActive)
        {
            obstacleRb.AddForce(zSpeed * Vector3.forward, ForceMode.Impulse);
            obstacleRb.AddForce(xSpeed * Vector3.right, ForceMode.Impulse);
            transform.position =
                new(transform.position.x, yOffSet, transform.position.z);

        }
        // Destroy an object after moving past the camera's view
        if (transform.position.z < zLowBound || transform.position.z > zHighBound
        || transform.position.x < -xBound || transform.position.x > xBound) {
            Destroy(gameObject);
        }
    }
    // Set axis speed of obstacle spawning from either edge and corner
    void SetAxisSpeed()
    {
        if (transform.position.x == xBound)
        {
            xSpeed = -moveSpeed;
        }
        else if (transform.position.x == -xBound)
        {
            xSpeed = moveSpeed;
        }

        if (transform.position.z == zLowBound)
        {
            zSpeed = moveSpeed;
        }
        else if (transform.position.z == zHighBound)
        {
            zSpeed = -moveSpeed;
        }
    }
    // Set the obstacle volume based on the SFX Volume Setting
    void SetObstacleAudio()
    {
        obstacleAudio.volume = gameManager.SFXVolume;
    }
    // Ramp up the obstacle's speed by the highest current score
    void RampSpeed()
    {
        float ramp = (gameManager.P1Score > gameManager.P2Score)? 10 * gameManager.P1Score: 10 * gameManager.P2Score;
        moveSpeed += ramp;
    }
    // OnCollisionEnter is called when an obstacle collides with a tree
    private void OnCollisionEnter(Collision collision)
    {
        _ = GameObject.FindGameObjectWithTag("Ground");
        if (collision.gameObject.name.Equals("Tree"))
        {
            transform.Translate(-1, -1, 0);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            obstacleAudio.Play();
        }
        if (collision.gameObject.CompareTag("Weapon"))
        {
            Destroy(gameObject);
        }
    }
}
