using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorScript : MonoBehaviour
{

    [Header("Standard Settings")]
    public GameObject usedButton;
    ButtonScript bs;

    public float speed;
    private Vector2 direction;

    [Header("Custom Settings")]
    [Tooltip("If true, doesn't require a button to activate, and is 'on' the entire time.")]
    public bool alwaysActive;

    private string playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        if (!alwaysActive)
        {
            bs = usedButton.GetComponent<ButtonScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (alwaysActive || bs.buttonPressed)
        {

            Debug.Log("In Collision with: " + collision.gameObject.name);
            direction = transform.right;
            direction = direction * speed;
                Rigidbody2D collisionRB = collision.gameObject.GetComponent<Rigidbody2D>();

                if (collisionRB)
                {
                    collisionRB.velocity = Vector2.zero;
                    Debug.Log("Found RigidBody2D");
                   
                    //collisionRB.AddForce(direction * collisionRB.mass);
                    collisionRB.MovePosition(collisionRB.position + (Vector2)(gameObject.transform.right * (speed * Time.fixedDeltaTime)));
                }
            
        }
    }

}
