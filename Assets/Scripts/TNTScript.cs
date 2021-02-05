using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTScript : MonoBehaviour
{
   
    Rigidbody2D rb;
    private bool canExplode = false;

    public float power = 10.0F;
    public float timeRemainingMax = .05f;

    private float timeRemaining = 2;


    public ParticleSystem explosion1;
    public ParticleSystem explosion2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeRemaining = timeRemainingMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (canExplode)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }

            if (timeRemaining < 0)
            {
                if (SFXManager.main)
                {
                    SFXManager.main.TNTExplode();
                }
                CraneManagement.main.LaunchCrates(this.transform.position, power);
                explosion1.Play();
                explosion2.Play();
                //Destroy(this.gameObject);
                gameObject.SetActive(false);
                canExplode = false;
                timeRemaining = timeRemainingMax;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collisionRB = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collisionRB && collision.gameObject.layer != 13)
        {
            canExplode = true;

        }
        
    }


}
