using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject ropeLinkObj;

    public CraneManagement craneManagement;

    public int linkCount;

    void Start()
    {
        GenerateRope();
    }

    void GenerateRope()
    {
        Rigidbody2D prevRB = hook;
        for (int i = 0; i < linkCount; i++)
        {
            GameObject newLink = Instantiate(ropeLinkObj, transform);
            HingeJoint2D _joint = newLink.GetComponent<HingeJoint2D>();
            _joint.connectedBody = prevRB;


            if (i >= linkCount - 1)
            {
                craneManagement.AttachToRopeEnd(newLink.GetComponent<Rigidbody2D>());
            }
            else
            {
                prevRB = newLink.GetComponent<Rigidbody2D>();
            }

            
        }
    }

}
