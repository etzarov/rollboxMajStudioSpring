using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject ropeLinkObj;

    public CraneManagement craneManagement;

    [Header("Rope Swaying")]
    public float angularDragDefault;
    public float ropeLimitDefault;
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


            float angleLimitMult = 7f / (float)linkCount;

            Rigidbody2D rbLink = newLink.GetComponent<Rigidbody2D>();

            rbLink.angularDrag = angularDragDefault * Mathf.Pow(angleLimitMult,2);

            HingeJoint2D _joint = newLink.GetComponent<HingeJoint2D>();
            _joint.connectedBody = prevRB;
            _joint.useLimits = true;
            JointAngleLimits2D angleLimits = _joint.limits;
            

            angleLimits.min = -ropeLimitDefault * angleLimitMult;
            angleLimits.max = ropeLimitDefault * angleLimitMult;
            _joint.limits = angleLimits;
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
