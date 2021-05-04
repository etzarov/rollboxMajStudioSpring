using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas : MonoBehaviour
{
    private Rigidbody2D rbody;
    [SerializeField] private bool killplayer = false;
    [SerializeField] private float deathtime = 5.0f;
    [SerializeField] private AudioClip damagesound;

    private AudioSource myaudio;
    private float deathtimer = -1;
    private float audiotimer = 0f;
    private float audiospan = 1.5f;
    private void Start()
    {
        if (killplayer)
        {
            myaudio = this.GetComponent<AudioSource>();
        }
        rbody = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!killplayer || CurrentLevelManager.main.Completed())
            return;
        
        if(deathtimer > 0)
        {
            audiotimer -= Time.deltaTime;
            if (audiotimer <= 0 && !myaudio.isPlaying) {
                audiotimer = audiospan;
                myaudio.PlayOneShot(damagesound,0.5f);
            }
            deathtimer -= Time.deltaTime;
            if(deathtimer <= 0)
            {
                CurrentLevelManager.main.ResetGame();
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            deathtimer = -1;
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            deathtimer = deathtime;
        }
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
