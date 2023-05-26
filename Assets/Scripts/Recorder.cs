using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    private bool recording;
    public bool Recording
    {
        set
        {
            recording = value;
            if (!value)
            {
                gameObject.GetComponent<GameManager>().pauseAnimations();
                dataObject.LeftHandQuaternions = leftHandRotations;
                dataObject.LeftHandPositions = leftHandPositions;
                dataObject.RightHandQuaternions = rightHandRotation;
                dataObject.RightHandPositions = rightHandPositions;
                List<Vector3> lEuler = new List<Vector3>();
                List<Vector3> rEuler = new List<Vector3>();
                leftHandRotations.ForEach(x => lEuler.Add(x.eulerAngles));
                rightHandRotation.ForEach(x => rEuler.Add(x.eulerAngles));
                dataObject.LeftHandEuler = lEuler;
                dataObject.RightHandEuler = rEuler;
                dataObject.VRRig = GameObject.Find("XR Origin").transform;
                dataObject.ExportToJson(folder);
                gameObject.GetComponent<GameManager>().moveToNextClip();
            }
            else
            {
                Debug.Log("Start recording");

                dataObject = new DataSetObject(to,toTransfrom);
                leftHandPositions.Clear();
                rightHandPositions.Clear();
                leftHandRotations.Clear();
                rightHandRotation.Clear();

            }
        }
    }
    public Transform leftHand;
    public Transform rightHand;
    private DataSetObject dataObject;
    public AnimationClip  to;// to be setup by game manager
    public string folder; // to be set up by game manager

    private List<Quaternion> leftHandRotations = new List<Quaternion>();
    private List<Quaternion> rightHandRotation = new List<Quaternion>();
    private List<Vector3> rightHandPositions = new List<Vector3>();
    private List<Vector3> leftHandPositions = new List<Vector3>();
    public Transform toTransfrom;//same

    private void Start()
    {
        leftHand = GameObject.Find("LeftController").transform;
        rightHand = GameObject.Find("RightController").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (recording)
        {
            //position recording
            rightHandPositions.Add(rightHand.parent.localPosition);
            leftHandPositions.Add(leftHand.parent.localPosition);

            // rotation recording
            rightHandRotation.Add(rightHand.parent.localRotation);
            leftHandRotations.Add(leftHand.parent.localRotation);
        }
    }

}
