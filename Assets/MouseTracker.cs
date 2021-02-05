using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    Vector3 targetPos;

    void Update()
    {
        
        targetPos = targetPos.GetInputPosition();
        transform.position = targetPos;
    }
}
