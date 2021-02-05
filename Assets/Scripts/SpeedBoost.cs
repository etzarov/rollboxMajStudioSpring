using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public ButtonScript bs;
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public GameObject button;
    public GameObject playerBall;
    public bool alwaysOn = false;

    public Sprite sbOn;
    public Sprite sbOff;

    public float thrust = 1f;

    private bool onBoost = false;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!alwaysOn)
        {
            bs = button.GetComponent<ButtonScript>();
        }

        sr = GetComponent<SpriteRenderer>();
        rb = playerBall.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alwaysOn || bs.buttonPressed)
        {
            this.sr.sprite = sbOn;

            if (onBoost)
            {
                rb.AddForce(transform.right * thrust);
            }

        }
        else
        {
            this.sr.sprite = sbOff;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == playerBall)
        {
            onBoost = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == playerBall)
        {
            onBoost = false;
        }
    }
}
