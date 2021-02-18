using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    public ButtonScript bs;
    public GameObject button;
    private bool isRotating = true;

    public float thrust = 1f;

    // Start is called before the first frame update
    void Start()
    {
        bs = button.GetComponent<ButtonScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bs.buttonPressed)
        {
            isRotating = true;
            if (isRotating)
            {
                Vector3 to = new Vector3(0, 0, 0);
                if (Vector3.Distance(transform.eulerAngles, to) > 0.01f)
                {
                    transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, -to, Time.deltaTime * thrust);
                }
                else
                {
                    transform.eulerAngles = to;
                    isRotating = false;
                }
            }
        }

        else
        {
            isRotating = false;
        }
    }
}



