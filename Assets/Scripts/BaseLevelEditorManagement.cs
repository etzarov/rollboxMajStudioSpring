using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelEditorManagement : MonoBehaviour
{
    void Start()
    {
        UpdateZPos();
    }

    /// <summary>
    /// Updates Z pos of an object so that it remains on the right plane.
    /// </summary>
    void UpdateZPos()
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }
}
