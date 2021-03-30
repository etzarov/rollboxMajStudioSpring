using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour
{
    private Rigidbody2D rbody;

    private void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Crate Hatch(Clone)" && 
            
            collision.gameObject.layer == 9 &&
            this.transform.position.y < collision.gameObject.transform.position.y)
        {

            if (this.transform.position.x < collision.gameObject.transform.position.x)
            {
                rbody.AddForce(new Vector2(-2.0f,0), ForceMode2D.Impulse);
            }
            else
            {
                rbody.AddForce(new Vector2(2.0f, 0), ForceMode2D.Impulse);
            }
        }
    }
}
