using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject ropeLinkObj;

    public GameObject connectorRB;
    public float distanceToEnd;
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
                AttachToRopeEnd(newLink.GetComponent<Rigidbody2D>());
            }
            else
            {
                prevRB = newLink.GetComponent<Rigidbody2D>();
            }

            
        }
    }

        public void AttachToRopeEnd(Rigidbody2D endRB)
    {
        HingeJoint2D joint = connectorRB.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;

        joint.connectedBody = endRB;

        joint.anchor = Vector2.zero;
        joint.useLimits = true;
        JointAngleLimits2D angleLimits = joint.limits;
        angleLimits.min = -25;
        angleLimits.max = 25;
        joint.limits = angleLimits;
        joint.connectedAnchor = new Vector2(0f, -distanceToEnd);
    }


}
