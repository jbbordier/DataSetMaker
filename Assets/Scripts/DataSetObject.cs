using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class DataSetObject
{
    public List<Quaternion> LeftHandQuaternions = new List<Quaternion>();
    public List<Quaternion> RightHandQuaternions = new List<Quaternion>();
    public List<Vector3> LeftHandEuler = new List<Vector3>();
    public List<Vector3> LeftHandPositions = new List<Vector3>();
    public List<Vector3> RightHandEuler = new List<Vector3>();
    public List<Vector3> RightHandPositions = new List<Vector3>();
    public AnimationClip clipFrom;
    public AnimationClip clipTo;
    public Transform transformFrom;
    public Transform transformTo;


    // animationDatas
    private List<Quaternion> rotLocal = new List<Quaternion>();
    private List<Quaternion> rotGlobal = new List<Quaternion>(); 
    private List<Vector3> posLocal = new List<Vector3>();
    private List<Vector3> posGlobal = new List<Vector3>(); 



    public DataSetObject(AnimationClip from, AnimationClip to,Transform trFrom, Transform trTo)
    {
        clipFrom = from;
        clipTo = to;
        transformFrom = trFrom;
        transformTo = trTo;
    }
    public void ExportToJson(string folder)
    {
        using (StreamWriter writer = new StreamWriter(folder+"/datasAnim.txt"))
        {
            //animations names from --> to
            writer.WriteLine("Animations :");
            writer.WriteLine(clipFrom.name + " and " + clipTo.name);


            //Quaternions
            writer.WriteLine("LeftHand quaternion :");
            writeList<Quaternion>(writer, LeftHandQuaternions);
            writer.WriteLine("RightHand quaternion :");
            writeList<Quaternion>(writer, RightHandQuaternions);
            
            //Eulers
            /*
            writer.WriteLine("LeftHand euler :");
            writeList<Vector3>(writer, LeftHandEuler);
            writer.WriteLine("RightHand euler :");
            writeList<Vector3>(writer, RightHandEuler);*/

            //Positions
            writer.WriteLine("LeftHand pos :");
            writeList<Vector3>(writer, LeftHandPositions);
            writer.WriteLine("RightHand pos :");
            writeList<Vector3>(writer, RightHandPositions);

            //From Anim rotations 
            ExtractAnimationData(clipFrom, transformFrom);
            writer.WriteLine("From animation Local Rotations : ");
            writeList<Quaternion>(writer, rotLocal);
            writer.WriteLine("From animation Global Rotations : ");
            writeList<Quaternion>(writer, rotGlobal);


            //From Anim positions
            writer.WriteLine("From animation Local Positions : ");
            writeList<Vector3>(writer, posLocal);
            writer.WriteLine("From animation Global Positions : ");
            writeList<Vector3>(writer, posGlobal);

            //To Anim rotations 
            ExtractAnimationData(clipTo, transformTo);
            writer.WriteLine("To animation Local Rotations : ");
            writeList<Quaternion>(writer, rotLocal);
            writer.WriteLine("To animation Global Rotations : ");
            writeList<Quaternion>(writer, rotGlobal);


            //To Anim positions
            writer.WriteLine("To animation Local Positions : ");
            writeList<Vector3>(writer, posLocal);
            writer.WriteLine("To animation Global Positions : ");
            writeList<Vector3>(writer, posGlobal);

        }
    }

    internal void ExportToTensor(string folder)
    {
      
    }

    private void writeList<T>(StreamWriter writer, List<T> list)
    {
        foreach (var item in list)
        {
            writer.Write(item.ToString() + ';');
        }
        writer.WriteLine();
    }

    private void ExtractAnimationData(AnimationClip animationClip,Transform targetTransform)
    {
        posGlobal.Clear();
        posLocal.Clear();
        rotGlobal.Clear();
        rotLocal.Clear();

        Transform[] childs = targetTransform.GetComponentsInChildren<Transform>(true);
        ExtractAnimPerTransform(animationClip, targetTransform);
        for(int i = 0; i < childs.Length; i++)
        {
            ExtractAnimPerTransform(animationClip, childs[i]);
        }
        
    }



    void ExtractAnimPerTransform(AnimationClip animationClip, Transform targetTransform)
    {
        // Get the number of frames in the animation clip
        int frameCount = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);

        // Extract the position and rotation of each frame
        for (int i = 0; i < frameCount; i++)
        {
            float time = i / animationClip.frameRate;

            // Get the position and rotation of the target object at the current frame
            Vector3 position = targetTransform.position;
            Quaternion rotation = targetTransform.rotation;

            // Sample the animation clip at the current time
            animationClip.SampleAnimation(targetTransform.gameObject, time);

            // Get the new position and rotation of the target object after the animation clip has been sampled

            //Pos
            Vector3 localPosition = targetTransform.localPosition;
            Vector3 globalPosition = targetTransform.position;

            //Rot
            Quaternion localRotation = targetTransform.localRotation; 
            Quaternion globalRotation = targetTransform.rotation;

            //Pos
            posLocal.Add(localPosition);
            posGlobal.Add(globalPosition);
            //Rot
            rotLocal.Add(localRotation);
            rotGlobal.Add(globalRotation);
            

            // Reset the position and rotation of the target object to their original values
            targetTransform.position = position;
            targetTransform.rotation = rotation;
        }
    }
}


