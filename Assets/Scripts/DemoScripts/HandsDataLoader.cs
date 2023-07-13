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
    public GameObject lPivot;
    public GameObject rPivot;

    List<Quaternion> lRot = new List<Quaternion>();
    List<Quaternion> rRot = new List<Quaternion>();
    List<Vector3> lPos = new List<Vector3>();
    List<Vector3> rPos = new List<Vector3>();

    [ContextMenu("Load")]
    public void LoadAnim()
    {

        List<Quaternion> lRot = new List<Quaternion>();
        List<Quaternion> rRot = new List<Quaternion>();
        List<Vector3> lPos = new List<Vector3>();
        List<Vector3> rPos = new List<Vector3>();

        getHandsPositionRotation(pathAnimTesting, "", lPos, rPos, lRot, rRot);
        DrawHandTrajectory(lPos, rPos, lRot, rRot, new Color(255, 0, 0), new Color(100, 0, 0));

        lRot = new List<Quaternion>();
        rRot = new List<Quaternion>();
        lPos = new List<Vector3>();
        rPos = new List<Vector3>();

        getHandsPositionRotation(pathAnimTraining, animName, lPos, rPos, lRot, rRot);
        DrawHandTrajectory(lPos, rPos, lRot, rRot, new Color(0, 255, 0), new Color(0, 100, 0));

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
                DrawHandTrajectory(lPos, rPos, lRot, rRot, colorL, colorR);
            }
        }
    }

    private void DrawHandTrajectory(List<Vector3> lPos, List<Vector3> rPos, List<Quaternion> lRot, List<Quaternion> rRot, Color lcolor, Color rcolor)
    {
        GameObject lineL = new GameObject();
        lineL.transform.parent = lPivot.transform;
        lineL.transform.position = Vector3.zero;
        GameObject lineR = new GameObject();
        lineR.transform.parent = rPivot.transform;
        lineR.transform.position = Vector3.zero;
        LineRenderer lineRendererL = lineL.AddComponent<LineRenderer>();
        lineRendererL.useWorldSpace = false;
        lineRendererL.material = new Material(Shader.Find("Standard"));
        lineRendererL.transform.parent = lPivot.transform;
        lineRendererL.positionCount = lPos.Count;
        lineRendererL.startWidth = 0.05f;
        lineRendererL.endWidth = 0.05f;
        lineRendererL.material.color = lcolor;
        LineRenderer lineRendererR = lineR.AddComponent<LineRenderer>();
        lineRendererR.useWorldSpace = false;
        lineRendererR.transform.parent = rPivot.transform;
        lineRendererR.positionCount = rPos.Count;
        lineRendererR.startWidth = 0.05f;
        lineRendererR.endWidth = 0.05f;
        lineRendererR.material.color = rcolor;
        for (int i = 0; i < lPos.Count; i++)
        {
            lineRendererL.SetPosition(i, lPos[i]);
            lineRendererR.SetPosition(i, rPos[i]);
        }

    }

    void getHandsPositionRotation(string path, string animName, List<Vector3> lposition, List<Vector3> rposition, List<Quaternion> lrotation, List<Quaternion> rrotation)
    {

        using (StreamReader reader = new StreamReader(pathAnimTesting))
        {
            string content = reader.ReadToEnd();
            string[] anim = content.Split("Animations :");
            int indexOfChosenAnim = 0;
            if (animName != "")
            {
                for (int i = 0; i < anim.Length; i++)
                {
                    if (anim[i].Contains(animName))
                    {
                        indexOfChosenAnim = i;
                    }
                }
            }
            string[] frame = anim[indexOfChosenAnim].Split("\n");
            string[] leftQuat = frame[2].Split(';');
            string[] rightQuat = frame[4].Split(';');
            string[] leftPos = frame[6].Split(';');
            string[] rightPos = frame[8].Split(';');

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
