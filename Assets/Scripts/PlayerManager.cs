using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager main;

    public Rigidbody2D rb;
    Vector3 initPos;

    [Tooltip("Max speed the player can be moving on level complete platform to win.")]
    public float maxSpeedToCompleteLevel;
    public float timeToCompleteLevel;

    float totalTime;


    float magStore;

    private void Awake()
    {
        main = this;
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initPos = rb.position;
    }

    private void FixedUpdate()
    {
        magStore = rb.velocity.magnitude;
    }

    public void ResetPlayer()
    {
        if (CraneManagement.main.firstCratePlayer)
        {
            Destroy(this.gameObject);
        }
        rb.position = initPos;
        rb.angularVelocity = 0;
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            if (CurrentLevelManager.main != null)
            {
                CurrentLevelManager.main.mustReset = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            
            if (rb.velocity.magnitude < maxSpeedToCompleteLevel)
            {
                totalTime += Time.deltaTime;
                if (totalTime >= timeToCompleteLevel)
                {
                    if (CurrentLevelManager.main != null)
                    {
                        CurrentLevelManager.main.LevelCompleted();
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        totalTime = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SFXManager.main)
        {
            SFXManager.main.PlayerHitSound(magStore);
        }
    }
}
