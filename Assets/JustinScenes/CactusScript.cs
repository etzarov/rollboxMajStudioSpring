using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collisionRB = collision.gameObject.GetComponent<Rigidbody2D>();
        CrateInfo crateInfo = collision.gameObject.GetComponent<CrateInfo>();

        if (crateInfo != null)
        {
            if (collision.gameObject.layer == 9)
            {
                Debug.Log("breaking crate!");
                CrateSpawner.main.BreakCrate(crateInfo);
            }
        }

        if(collision.gameObject.layer == 12)
        {
            CurrentLevelManager.main.ResetGame();
        }

    }


    //Added by Taiyo Baniecki for TNT Crate Explosion
    //Edited by Leo Wang to solve "Destroying assets is not permitted to avoid data loss."
    //It how disables the object instead of destroying it
    public void DestroySelf()
    {
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
