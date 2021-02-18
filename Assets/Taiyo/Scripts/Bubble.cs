using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] float duration = 3.0f;
    [SerializeField] float speed = 1.0f;
    private float timer;
    private Transform catchObj = null;

    private void Start()
    {
        timer = duration;
    }


    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = duration;
            speed *= -1;
        }
        this.transform.Translate(speed*Time.deltaTime, 0, 0);

        if (catchObj != null)
        {
            catchObj.position = this.transform.position;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (catchObj == null)
        {
            catchObj = collision.gameObject.transform;
        }
        else
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
