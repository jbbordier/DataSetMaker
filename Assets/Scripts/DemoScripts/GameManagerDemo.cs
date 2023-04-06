using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerDemo : MonoBehaviour
{
    private Transform leftHand;
    private Transform rightHand;
    private List<Quaternion> leftHandRotations = new List<Quaternion>();
    private List<Quaternion> rightHandRotations = new List<Quaternion>();
    private List<Vector3> rightHandPositions = new List<Vector3>();
    private List<Vector3> leftHandPositions = new List<Vector3>();

    private bool recording;
    public bool Recording
    {
        set
        {
            recording = value;
            if (!value)
            {
                //not recording anymmore, need to load model etc
            }
            else
            {
                Debug.Log("Start recording");

                leftHandPositions.Clear();
                rightHandPositions.Clear();
                leftHandRotations.Clear();
                rightHandRotations.Clear();

            }
        }
    }
    void Start()
    {
        leftHand = GameObject.Find("LeftController").transform;
        rightHand = GameObject.Find("RightController").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (recording)
        {
            leftHandPositions.Add(leftHand.position);
            leftHandRotations.Add(leftHand.rotation);
            rightHandPositions.Add(rightHand.position);
            rightHandRotations.Add(rightHand.rotation);
        }
    }
}
