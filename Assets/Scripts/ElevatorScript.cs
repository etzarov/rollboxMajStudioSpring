using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ElevatorScript : MonoBehaviour
{
    

    
    ButtonScript bs;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Vector2 PlatStart;
    
   
    [Header("Standard Settings")]
    public GameObject usedButton;
    public SpriteRenderer spriteRenderer;
    public float speed = .1f;

    [Header("Custom Settings")]
    [Tooltip("If true, doesn't require a button to activate, and is 'on' the entire time.")]
    public bool alwaysActive;
    [Tooltip("If true, moves back and forth between starting and ending position.")]
    public bool oscillatesPosition;
    [Tooltip("If true, platform will return to its original position when the button stops being pushed.")]
    public bool retractsWhenTurnedOff;
    [Space]
    public Vector2 PlatEnd;
    private float distBetween;
    private float distBetween2;
    bool flip = false;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (usedButton != null)
        {
            Gizmos.DrawRay(transform.position, (usedButton.transform.position-transform.position) - (usedButton.transform.position - transform.position).normalized*.25f);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(PlatStart, PlatEnd);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PlatStart, .1f);
        Gizmos.DrawWireSphere(PlatEnd, .1f);
    }

    void Start()
    {
        
        bs = usedButton.GetComponent<ButtonScript>();
        rb = GetComponent<Rigidbody2D>();
        PlatStart = rb.position;
    }

    void FixedUpdate()
    {
        if (bs.buttonPressed || alwaysActive)
        {
            print("true");
            distBetween = Vector2.Distance(rb.position, PlatEnd);
            distBetween2 = Vector2.Distance(rb.position, PlatStart);

            if (!flip)
            {
                Vector2 targetDeltaMove = PlatEnd - rb.position;
                targetDeltaMove = targetDeltaMove.normalized * speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + targetDeltaMove);

                spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(1), 1, 1);
            }
            else if (flip && oscillatesPosition)
            {
                Vector2 targetDeltaMove = PlatStart - rb.position;
                targetDeltaMove = targetDeltaMove.normalized * speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + targetDeltaMove);

                spriteRenderer.transform.localScale = new Vector3(-Mathf.Abs(1), 1, 1);
            }

            if (distBetween < .2f && !flip)
            {
                    flip = true;
            }

            if (distBetween2 < .2f && flip)
            {
                    flip = false;
            }


        }
        else if (retractsWhenTurnedOff)
        {
            flip = false;
            Vector2 targetDeltaMove = PlatStart - rb.position;
            targetDeltaMove = targetDeltaMove.normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + targetDeltaMove);

            spriteRenderer.transform.localScale = new Vector3(-Mathf.Abs(1), 1, 1);
        }
    }


    public void CreateEndPosition()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        PlatEnd = rb.position;
    }
}


