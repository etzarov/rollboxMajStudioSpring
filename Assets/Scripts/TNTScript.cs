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



    //Added by Taiyo Baniecki

    //The max distance which the objects will get destroyed by TNT     
    [Range(1,10)][SerializeField] private float explodeDistance = 2.5f;

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
                //CraneManagement.main.LaunchCrates(this.transform.position, power);
                Launch(power);
                DestoryWhenExplode(explodeDistance);
                explosion1.Play();
                explosion2.Play();
                //Destroy(this.gameObject);
                gameObject.SetActive(false);
                canExplode = false;
                timeRemaining = timeRemainingMax;
            }
        }
    }

    public void Launch(float power)
    {
        Vector2 baseVal = new Vector2(1, 1);
        Vector2 distPL = PlayerManager.main.transform.position - this.transform.position;
        PlayerManager.main.rb.AddForce(power * distPL.normalized / Mathf.Pow(distPL.magnitude, 2f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Destory objects with certain tags
    /// when it is within the range of dist
    /// </summary>
    /// <param name="dist"></param>
    public void DestoryWhenExplode(float dist)
    {
        //For crate deletion
        CrateInfo[] cos;
        cos = Resources.FindObjectsOfTypeAll(typeof(CrateInfo)) as CrateInfo[];

        Vector3 p = this.transform.position;

        foreach(CrateInfo co in cos)
        {
            if (Vector3.Distance(co.transform.position,p) < dist)
            {
                co.BreakCrate(false);
                co.DestroySelf();
            }
        }



        //For elevator deletion
        ElevatorScript[] eos;
        eos = Resources.FindObjectsOfTypeAll(typeof(ElevatorScript)) as ElevatorScript[];

        foreach (ElevatorScript eo in eos)
        {
            if (Vector3.Distance(eo.transform.position, p) < dist)
            {
                eo.DestroySelf();
            }
        }

        //TODO : Add cactus deletion
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
