using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeballManager : MonoBehaviour
{
    [Header("Pupil Management")]
    public Vector2 pupilLocalCenter;
    public float pupilMoveRadius;
    public Transform pupil;
    [Space]
    public float maxChangeLookSpeed; //The max speed the ball can go and still change it's eye position
    Vector2 targetLookDirection;
    Rigidbody2D rb;
    [Space]
    public LayerMask importantMask; //Objects on important layers will be looked at when nearby instead of the crane/dropped crate.
    public float lookImportantRadius; //The distance from the exit to start looking at it.
    bool shouldLookAtExit;

    [Header("Blink Management")]
    public SpriteRenderer blinkSR;
    public Sprite[] blinkSprites;
    [Space]
    public float blinkTime;
    public float timeBetweenBlinks;
    float _cBlinkTime;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        _cBlinkTime = timeBetweenBlinks + Random.Range(0, 4);
        targetLookDirection = new Vector2(1, 1);
    }


    void Update()
    {
        //Update pupil
        UpdatePupilPosition();
        UpdateTargetPosition();

        //Update blinking
        _cBlinkTime -= Time.deltaTime;
        if (_cBlinkTime < 0)
        {
            _cBlinkTime = timeBetweenBlinks + Random.Range(0, 4);
            StartCoroutine(PlayerBlink());
        }
    }


    
    public IEnumerator PlayerBlink()
    {
        blinkSR.enabled = true;
        float t = 0;
        while (t < blinkTime/2)
        {
            int blinkFrame = Mathf.FloorToInt(t.Remap(0, blinkTime / 2, 0, blinkSprites.Length));
            blinkSR.sprite = blinkSprites[blinkFrame];

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        while (t < blinkTime)
        {
            int blinkFrame = Mathf.FloorToInt(t.Remap(blinkTime/2, blinkTime, blinkSprites.Length, 0));
            blinkSR.sprite = blinkSprites[blinkFrame];

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        blinkSR.enabled = false;
    }
    

    void UpdatePupilPosition()
    {
        bool shouldChangeLookDirection = false;
        if (rb.velocity.magnitude < maxChangeLookSpeed)
        {
            shouldChangeLookDirection = true;
        }

        if (ShouldLookAtExit() && rb.velocity.magnitude < maxChangeLookSpeed * 2f)
        {
            shouldChangeLookDirection = true;
        }

        if (shouldChangeLookDirection)
        {
            Vector2 prevLocalPosition = pupil.localPosition;
            Vector2 currentWorldPosition = pupil.position;
            pupil.localPosition = pupilLocalCenter;
            
            Vector2 targetWorldPosition = currentWorldPosition + (targetLookDirection.normalized * pupilMoveRadius);
            pupil.localPosition = prevLocalPosition;
            
            pupil.position = Vector2.Lerp(currentWorldPosition,targetWorldPosition,.05f);

            Vector2 pupilOffset = (Vector2)pupil.localPosition - pupilLocalCenter;
            if (pupilOffset.magnitude > pupilMoveRadius)
            {
                pupil.localPosition = pupilLocalCenter + pupilOffset.normalized * pupilMoveRadius;
            }
        }
    }

    void UpdateTargetPosition()
    {
        if (CraneManagement.main != null)
        {
            if (CraneManagement.main.heldCrate != null && !CraneManagement.main.dropDisabled)
            {
                Vector2 newTarget = Vector2.zero;
                float moveSpeed = .005f;

                if (!CraneManagement.main.holdingCrate)
                {
                    moveSpeed = .1f;
                }
                newTarget = CraneManagement.main.heldCrate.position;

                Vector2 targetVector = newTarget - rb.position;
                targetLookDirection = Vector2.Lerp(targetLookDirection, targetVector.normalized, moveSpeed);
            }
        }

        if (ShouldLookAtExit())
        {
            Collider2D hit = Physics2D.OverlapCircle(rb.position, lookImportantRadius, importantMask);
            Vector2 targetVector = (Vector2)hit.transform.position - rb.position;
            targetLookDirection = Vector2.Lerp(targetLookDirection, targetVector.normalized, .01f);
        }
        
    }

    bool ShouldLookAtExit()
    {
        
        return Physics2D.OverlapCircle(rb.position, lookImportantRadius, importantMask);
    }
}
