using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;

public class GameManagerDemo : MonoBehaviour
{
    private Transform leftHand;
    private Transform rightHand;
    private List<Quaternion> leftHandRotations = new List<Quaternion>();
    private List<Quaternion> rightHandRotations = new List<Quaternion>();
    private List<Vector3> rightHandPositions = new List<Vector3>();
    private List<Vector3> leftHandPositions = new List<Vector3>();
    public AnimationClip clip;
    private bool recording;
    Server server;
    public GameObject guy;
    public float framerate = 60f;
    bool simulating = false;
    public bool web;
    public bool Recording
    {
        set
        {
            recording = value;
            if (!value)
            {

                if (!simulating)
                {
                    if (web)
                    {
                        server.enabled = true;
                        server.SendData(leftHandRotations, rightHandRotations, leftHandPositions, rightHandPositions);
                    }
                    else
                    {
                        Tester tester = gameObject.GetComponent<Tester>();
                        tester.writeFile(leftHandRotations, rightHandRotations, leftHandPositions, rightHandPositions);
                    }
                }
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
        server = gameObject.GetComponent<Server>();
    }
    [ContextMenu("SimulateRecording")]
    public void SimulateRecording()
    {
        simulating = true;
        Recording = false;
        HandsDataLoader handsLoader = gameObject.GetComponent<HandsDataLoader>();
        if (web)
            server.SendData(handsLoader.lRot, handsLoader.rRot, handsLoader.lPos, handsLoader.rPos);
        else
        {
            Tester tester = gameObject.GetComponent<Tester>();
            tester.writeFile(handsLoader.lRot, handsLoader.rRot, handsLoader.lPos, handsLoader.rPos);
        }
    }
    [ContextMenu("CloseServer")]
    public void CloseServer()
    {
        server.enabled = false;
    }

    [ContextMenu("SimulateAnimation")]
    public void SimulateAnimation()
    {
        gameObject.GetComponent<AnimationLoader>().Load();
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
        if (server.hasReceiveData)
        {
            server.enabled = false;
        }
    }

    // i want this fuction to modify the animationClip clip with the data from the server. First it need to convert the data to a dictionnary and then modify each animationCurve for each joints
    public void SetAnimation(List<List<Quaternion>> quaternions)
    {
        Dictionary<int, List<Quaternion>> dict = convertListToDictionnary(quaternions);
        Transform[] tr = guy.GetComponentsInChildren<Transform>();
        Transform root = guy.transform.parent;
        for (int i = 0; i < dict.Count; i++)
        {
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();
            AnimationCurve curveW = new AnimationCurve();
            string path = "";
            for (int j = 0; j < dict[i].Count; j++)
            {
                curveX.AddKey((j + 1) / framerate, dict[i][j].x);
                curveY.AddKey((j + 1) / framerate, dict[i][j].y);
                curveZ.AddKey((j + 1) / framerate, dict[i][j].z);
                curveW.AddKey((j + 1) / framerate, dict[i][j].w);
            }
            if (i == dict.Count - 1)
            {
                //here i need to modify the local position of the joint[i]
                path = AnimationUtility.CalculateTransformPath(tr[0], root);
                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.x", curveX);
                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.y", curveY);
                clip.SetCurve(path, typeof(Transform), "m_LocalPosition.z", curveZ);
            }
            else
            {
                path = AnimationUtility.CalculateTransformPath(tr[i], root);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.x", curveX);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.y", curveY);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.z", curveZ);
                clip.SetCurve(path, typeof(Transform), "m_LocalRotation.w", curveW);
            }


        }

        Animation anim = guy.transform.parent.GetComponent<Animation>();
        anim.AddClip(clip, "test");
        anim.Play("test");

    }

    public Dictionary<int, List<Quaternion>> convertListToDictionnary(List<List<Quaternion>> animation)
    {
        Dictionary<int, List<Quaternion>> dict = new Dictionary<int, List<Quaternion>>();
        for (int i = 0; i < animation.Count; i++)
        {
            for (int j = 0; j < animation[i].Count; j++)
            {
                if (dict.ContainsKey(j))
                {
                    dict[j].Add(animation[i][j]);
                }
                else
                {
                    dict.Add(j, new List<Quaternion>() { animation[i][j] });
                }
            }
        }
        return dict;
    }
}
