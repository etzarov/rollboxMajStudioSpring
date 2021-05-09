using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltSpriteAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] animSprites;
    [SerializeField] private float speed = 0.5f;
    private float timer = 0;
    private int animNum = 0;
    private int animCount = 0;
    SpriteRenderer mySprite;

    private void Start()
    {
        timer = speed;
        animNum = animSprites.Length;
        mySprite = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            animCount++;
            timer = speed;
            mySprite.sprite = animSprites[animCount % animNum];

        }
    }



}
