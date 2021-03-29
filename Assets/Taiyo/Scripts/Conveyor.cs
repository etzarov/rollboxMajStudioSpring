using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Conveyor : MonoBehaviour
{
    [SerializeField] private float speed = -2.0f;
    private List<Transform> targetTransform;
    private List<Rigidbody2D> targetRigidbody;
    private List<Vector3> initialPosition;

    private float leftend;
    private float rightend;


    private int entrypos = 0;
    private void Start()
    {
        targetTransform = new List<Transform>();
        targetRigidbody = new List<Rigidbody2D>();
        initialPosition = new List<Vector3>();


        var s = this.GetComponent<BoxCollider2D>().size;
        leftend = this.transform.localPosition.x - s.x/2;
        rightend = this.transform.localPosition.x + s.x/2;

    }
    private void Update()
    {
        if (targetTransform == null)
            return;

        int delete = -1;
        for (int i = 0; i < targetTransform.Count; i++)
        {
            if(targetTransform[i] == null)
            {
                continue;
            }

            

            if (speed < 0 && targetTransform[i].localPosition.x < leftend)
            {
                
                delete = i;
            }else if (speed >= 0 && targetTransform[i].localPosition.x > rightend)
            {
                delete = i;
            }else
            {
                initialPosition[i] += new Vector3(speed, 0, 0) * Time.deltaTime;
                targetTransform[i].position = initialPosition[i];
                Debug.Log("Down");
            }

            
        }

        if (delete != -1)
        {
            targetRigidbody[delete].velocity = new Vector2(3*Mathf.Sign(speed), 0);
            //targetRigidbody[delete].AddForce(new Vector2(-speed,0));
            targetTransform.RemoveAt(delete);
            initialPosition.RemoveAt(delete);
            targetRigidbody.RemoveAt(delete);
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (speed < 0) {
            
            if (collision.transform.localPosition.x < leftend) {

                return;
            }

        }
        else
        {

        }

        if (collision.gameObject.name == "Player")
        {
            targetTransform.Add(collision.transform);
            targetRigidbody.Add(collision.gameObject.GetComponent<Rigidbody2D>());
            initialPosition.Add(collision.transform.position);

        }
        else if(collision.gameObject.name == "Crate Hatch(Clone)")
        {
            targetTransform.Add(collision.transform);
            targetRigidbody.Add(collision.gameObject.GetComponent<Rigidbody2D>());
            initialPosition.Add(collision.transform.position);

        }
  
    }

} // class BeltConveyor
