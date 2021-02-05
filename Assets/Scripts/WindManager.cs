using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{

    public float raycastDistance = 5f;
    public float windSpeed = 5f;

    private string playerTag = "Player";
    public Rigidbody2D playerRB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit Hit;
        Debug.DrawRay(gameObject.transform.position, gameObject.transform.right * raycastDistance, Color.green, 2);

        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.right * raycastDistance, out Hit, raycastDistance))
        {
            if (Hit.transform.gameObject.CompareTag(playerTag))
            {
                playerRB.AddForce(transform.right * windSpeed, ForceMode2D.Impulse);
            }
        }
    }
}
