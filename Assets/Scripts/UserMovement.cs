using UnityEngine;

public class UserMovement : MonoBehaviour
{

    public UserBehaviour User;
    public bool isWorldGripped;

    private Vector3 previousPivot;
    private float previousDistance;
    private Vector3 previousForward;

    public void Start()
    {
        User.OnLeftGripPressed.AddListener(OnLeftHandGrip);
        User.OnLeftGripRelease.AddListener(OnLeftHandGripRelease);
        User.OnRightGripPressed.AddListener(OnRightHandGrip);
        User.OnRightGripRelease.AddListener(OnRightHandGripRelease);
    }

    public void FixedUpdate()
    {
        if (isWorldGripped)
        {
            Vector3 newPivot = (User.leftHand.transform.localPosition + User.rightHand.transform.localPosition) / 2f;
            transform.position += transform.TransformVector(previousPivot - newPivot);
            previousPivot = newPivot;

            //calculate new pivot to right hand vector
            Vector3 middleToRight = User.rightHand.transform.localPosition - previousPivot;
            //calculate newForward using middle to right as right axes and vector3.up as up axes
            Vector3 newForward = Vector3.Cross(middleToRight, Vector3.up);
            //calculate angle from previous forward to new forward around up axes
            float angle = Vector3.SignedAngle(previousForward, newForward, Vector3.up);
            //rotate transform around up axes of angle
            transform.RotateAround(transform.TransformPoint(newPivot), Vector3.up, -angle);
            previousForward = newForward;

            float newDistance = Vector3.Distance(User.leftHand.transform.localPosition, User.rightHand.transform.localPosition);
            float factor = previousDistance / newDistance;
            transform.localScale *= factor;
            previousDistance = newDistance;
        }
    }

    public void OnLeftHandGrip(Transform leftHand)
    {
        this.User.leftHand = leftHand;
        if (User.isRightGripPressed) StartGripWorld();
    }

    public void OnRightHandGrip(Transform rightHand)
    {
        this.User.rightHand = rightHand;
        if (User.isLeftGripPressed) StartGripWorld();
    }

    public void OnLeftHandGripRelease(Transform leftHand)
    {
        if (isWorldGripped) StopGripWorld();
    }
    public void OnRightHandGripRelease(Transform rightHand)
    {
        if (isWorldGripped) StopGripWorld();
    }

    public void StartGripWorld()
    {
        isWorldGripped = true;
        previousPivot = (User.leftHand.transform.localPosition + User.rightHand.transform.localPosition) / 2f;
        previousDistance = Vector3.Distance(User.leftHand.transform.localPosition, User.rightHand.transform.localPosition);
        //calculate pivot to right hand vector
        Vector3 middleToRight = User.rightHand.transform.localPosition - previousPivot;
        //calculate forward with middle to right as right axes and vector3.up as up axes
        previousForward = Vector3.Cross(middleToRight, Vector3.up);
    }

    public void StopGripWorld()
    {
        isWorldGripped = false;
    }

}
