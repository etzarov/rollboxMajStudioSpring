using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{

    [Header("Button State Info")]
    public bool buttonPressed = false;
    public bool toggleButton = false;
    [Space]
    [Tooltip("Objects that can push the button down")]
    public LayerMask pressableObjects;

    [Header("Components")]
    public Transform buttonTop;
    public Transform buttonMask;
    BoxCollider2D hitbox;

    bool pressedStore;

    private void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        float targetY = 0;
        float targetMaskY = -.2f;

        if (buttonPressed)
        {
            targetY = -.25f;
            targetMaskY = -.375f;
        }

        Vector2 cPos = buttonTop.localPosition;
        cPos.y = Mathf.Lerp(cPos.y, targetY, .1f);

        Vector2 cMaskPos = buttonMask.localPosition;
        cMaskPos.y = Mathf.Lerp(cMaskPos.y, targetMaskY, .1f);

        buttonTop.localPosition = cPos;
        buttonMask.localPosition = cMaskPos;
        CheckCollision();


        if (buttonPressed != pressedStore)
        {
            if (SFXManager.main)
            {
                SFXManager.main.ButtonOnOff(buttonPressed);
            }
        }

        pressedStore = buttonPressed;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    buttonPressed = true;
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!toggleButton)
    //    {
    //        buttonPressed = false;
    //    }
    //}


    void CheckCollision()
    {
        if (Physics2D.OverlapBox(transform.position, GetCollisionSize(), 0, pressableObjects))
        {
            buttonPressed = true;
        }
        else
        {
            if (!toggleButton)
            {
                buttonPressed = false;
            }
        }
    }


    Vector2 GetCollisionSize()
    {
        float xV = hitbox.size.x * transform.localScale.x;
        float yV = hitbox.size.y * transform.localScale.y;
        return new Vector2(xV, yV);
    }

}
