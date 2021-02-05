using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateInfo : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider touchHitbox;
    public GameObject[] breakEffects;
    [Space]
    [Header("Player Crate")]
    public bool holdingPlayer;
    public GameObject playerObj;

    float magStore;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        magStore = rb.velocity.magnitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            if (CraneManagement.main != null)
            {
                CraneManagement.main.DeleteCrate(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void BreakCrate(bool reset)
    {
        if (reset && holdingPlayer)
        {

        }
        else
        {
            GameObject crateBreak = Instantiate(breakEffects[Random.Range(0, breakEffects.Length)], transform.position, transform.rotation, null);
            crateBreak.transform.localScale = transform.lossyScale;
            if (holdingPlayer)
            {
                Instantiate(playerObj, transform.position, Quaternion.identity);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (SFXManager.main != null)
        {
            SFXManager.main.CrateHitSound(magStore);
        }
    }
}
