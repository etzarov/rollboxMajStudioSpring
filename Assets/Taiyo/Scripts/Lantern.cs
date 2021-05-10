using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    private float myScale;
    private Transform myRect;

    private float timer = 1.0f;
    [SerializeField] private float speed = 0.01f;

    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float minScale = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        myRect = this.GetComponent<Transform>();
        myScale = myRect.localScale.x;

    }

    // Update is called once per frame
    void Update()
    {
        myRect.localScale = myRect.localScale + myRect.localScale * speed;

        if(myRect.localScale.x > maxScale)
        {
            speed *= -1;
            myRect.localScale = new Vector3(maxScale, maxScale, maxScale);
        }
        
        if(myRect.localScale.x < minScale)
        {
            speed *= -1;
            myRect.localScale = new Vector3(minScale, minScale, minScale);
        }
    }
}
