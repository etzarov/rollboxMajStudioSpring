using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateAppear : MonoBehaviour
{
    public float appearSpeed;
    bool hasAppeared;
    float currentScale;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    
    void Update()
    {
        if (!hasAppeared)
        {
            currentScale += appearSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentScale;
            if (currentScale >= 1)
            {
                transform.localScale = Vector3.one;
                hasAppeared = true;
            }
        }
    }
}
