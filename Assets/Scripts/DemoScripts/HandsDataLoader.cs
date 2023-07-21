using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandsDataLoader : MonoBehaviour
{


    public string pathAnimTraining;
    public string pathAnimTesting = "Assets/Results/Tester/datasAnim.txt";
    public string animName;
    public int animFromTraining;
    GameObject trajHolder; 

    List<Quaternion> lRot = new List<Quaternion>();
    List<Quaternion> rRot = new List<Quaternion>();
    List<Vector3> lPos = new List<Vector3>();
    List<Vector3> rPos = new List<Vector3>();
    public int framenumber;

    private void Start()
    {
        trajHolder = GameObject.Find("TrajHolder");
    }

    [ContextMenu("Load")]
    public void LoadAnim()
    {
        
        int num = trajHolder.transform.childCount;
        while (num > 0)
        {
            DestroyImmediate(trajHolder.transform.GetChild(0).gameObject);
            num--;
        }
        List<Quaternion> lRot = new List<Quaternion>();
        List<Quaternion> rRot = new List<Quaternion>();
        List<Vector3> lPos = new List<Vector3>();
        List<Vector3> rPos = new List<Vector3>();

        getHandsPositionRotation(pathAnimTesting, "", lPos, rPos, lRot, rRot);
        DrawHandTrajectory(lPos, rPos, lRot, rRot, new Color(255, 0, 0), new Color(25, 0, 0),"test trajectory");

        lRot = new List<Quaternion>();
        rRot = new List<Quaternion>();
        lPos = new List<Vector3>();
        rPos = new List<Vector3>();

        getHandsPositionRotation(pathAnimTraining, animName, lPos, rPos, lRot, rRot);
        DrawHandTrajectory(lPos, rPos, lRot, rRot, new Color(0, 255, 0), new Color(0, 25, 0), "train trajectory");

    }

    [ContextMenu("DrawAllHands")]
    public void DrawAllHandsTrajectory()
    {
        using (StreamReader reader = new StreamReader(pathAnimTraining))
        {
            string content = reader.ReadToEnd();
            string[] anim = content.Split("Animations :");
            int animCount = anim.Length;
            for (int i = 1; i < anim.Length; i++)
            {
                lRot.Clear();
                rRot.Clear();
                lPos.Clear();
                rPos.Clear();
                string[] frame = anim[i].Split("\n");
                string[] leftQuat = frame[2 + 1].Split(';');
                string[] rightQuat = frame[4 + 1].Split(';');
                string[] leftPos = frame[6 + 1].Split(';');
                string[] rightPos = frame[8 + 1].Split(';');
                Color colorL = new Color(i * (255 / animCount), 0, 0);
                Color colorR = new Color(0, i * (255 / animCount), 0);
                for (int j = 0; j < leftQuat.Length - 1; j++)
                {
                    string[] quat = leftQuat[j].Split(',');
                    lRot.Add(new Quaternion(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")", "").Replace('.', ','))));
                    quat = rightQuat[j].Split(',');
                    rRot.Add(new Quaternion(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")", "").Replace('.', ','))));
                    quat = leftPos[j].Split(',');
                    lPos.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
                    quat = rightPos[j].Split(',');
                    rPos.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
                }
                DrawHandTrajectory(lPos, rPos, lRot, rRot, colorL, colorR, "train trajectory");
            }
        }
    }

    private void DrawHandTrajectory(List<Vector3> lPos, List<Vector3> rPos, List<Quaternion> lRot, List<Quaternion> rRot, Color lcolor, Color rcolor,string name)
    {
        GameObject lineL = new GameObject();
        lineL.transform.parent = trajHolder.transform;
        lineL.transform.position = Vector3.zero;
        lineL.name = name;
        GameObject lineR = new GameObject();
        lineR.transform.parent = trajHolder.transform;
        lineR.transform.position = Vector3.zero;
        lineR.name = name;
        LineRenderer lineRendererL = lineL.AddComponent<LineRenderer>();
        lineRendererL.useWorldSpace = false;
        lineRendererL.material = new Material(Shader.Find("Standard"));
        lineRendererL.transform.parent = trajHolder.transform;
        lineRendererL.positionCount = lPos.Count;
        lineRendererL.startWidth = 0.05f;
        lineRendererL.endWidth = 0.05f;
        lineRendererL.material.color = lcolor;
        LineRenderer lineRendererR = lineR.AddComponent<LineRenderer>();
        lineRendererR.useWorldSpace = false;
        lineRendererR.transform.parent = trajHolder.transform;
        lineRendererR.positionCount = rPos.Count;
        lineRendererR.startWidth = 0.05f;
        lineRendererR.endWidth = 0.05f;
        lineRendererR.material.color = rcolor;
        for (int i = 0; i < lPos.Count; i++)
        {
            if(i < lPos.Count)
            {
                lineRendererL.SetPosition(i, lPos[i]);
                lineRendererR.SetPosition(i, rPos[i]);
                if (i % 10 == 0)
                {
                    GameObject pos = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    pos.transform.parent = lineL.transform;
                    pos.transform.position = lPos[i];
                    pos.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    pos.GetComponent<Renderer>().material.color = Color.black;
                    pos.transform.localRotation = lRot[i];
                    pos.transform.position = pos.transform.position + pos.transform.forward * 0.05f;
                    pos.transform.Rotate(new Vector3(90, 0, 0));
                    pos = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    pos.transform.parent = lineR.transform;
                    pos.transform.position = rPos[i];
                    pos.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    pos.GetComponent<Renderer>().material.color = Color.gray;
                    pos.transform.localRotation = rRot[i];
                    pos.transform.position = pos.transform.position + pos.transform.forward * 0.05f;
                    pos.transform.Rotate(new Vector3(90, 0, 0));

                }
            }
            
        }

    }

    void getHandsPositionRotation(string path, string animName, List<Vector3> lposition, List<Vector3> rposition, List<Quaternion> lrotation, List<Quaternion> rrotation)
    {

        using (StreamReader reader = new StreamReader(path))
        {
            string content = reader.ReadToEnd();
            string[] anim = content.Split("Animations :");
            int indexOfChosenAnim = 1;
            int anim_with_name = 0;
            if (animName != "")
            {
                for (int i = 0; i < anim.Length; i++)
                {
                    var tab = anim[i].Split("\n");
                    if(tab.Length > 1)
                        if (anim[i].Split("\n")[1].Contains(animName))
                        {
                            if(anim_with_name == animFromTraining)
                            {
                                indexOfChosenAnim = i;
                                break;
                            }
                            else
                            {
                                anim_with_name++;
                            }
                        }
                }
            }
            string[] frame = anim[indexOfChosenAnim].Split("\n");
            string[] leftQuat = frame[2+1].Split(';');
            string[] rightQuat = frame[4+1].Split(';');
            string[] leftPos = frame[6+ 1].Split(';');
            string[] rightPos = frame[8 + 1].Split(';');

            for (int i = 0; i < leftQuat.Length - 1; i++)
            {
                string[] quat = leftQuat[i].Split(',');
                lrotation.Add(new Quaternion(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")", "").Replace('.', ','))));
                quat = rightQuat[i].Split(',');
                rrotation.Add(new Quaternion(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")", "").Replace('.', ','))));
                quat = leftPos[i].Split(',');
                lposition.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
                quat = rightPos[i].Split(',');
                rposition.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
            }

        }
    }


}
