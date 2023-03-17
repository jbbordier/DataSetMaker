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
    public AnimationClip clipTo;
    public Transform transformTo;


    // animationDatas
    private Dictionary<String, List<Vector3>> posLocal = new Dictionary<string, List<Vector3>>();
    private Dictionary<String, List<Vector3>> posGlobal = new Dictionary<string, List<Vector3>>();
    private Dictionary<String, List<Quaternion>> rotLocal = new Dictionary<string, List<Quaternion>>();
    private Dictionary<String, List<Quaternion>> rotGlobal = new Dictionary<string, List<Quaternion>>();




    public DataSetObject(AnimationClip to, Transform trTo)
    {
        clipTo = to;
        transformTo = trTo;
    }
    public void ExportToJson(string folder)
    {
        using (StreamWriter writer = new StreamWriter(folder+"/datasAnim.txt",true))
        {
            //animations names from --> to
            writer.WriteLine("Animations :");
            writer.WriteLine(clipTo.name);


            //Quaternions
            writer.WriteLine("LeftHand quaternion :");
            writeListOnly<Quaternion>(writer, LeftHandQuaternions);
            writer.WriteLine("RightHand quaternion :");
            writeListOnly<Quaternion>(writer, RightHandQuaternions);
            
            //Eulers
            /*
            writer.WriteLine("LeftHand euler :");
            writeList<Vector3>(writer, LeftHandEuler);
            writer.WriteLine("RightHand euler :");
            writeList<Vector3>(writer, RightHandEuler);*/

            //Positions
            writer.WriteLine("LeftHand pos :");
            writeListOnly<Vector3>(writer, LeftHandPositions);
            writer.WriteLine("RightHand pos :");
            writeListOnly<Vector3>(writer, RightHandPositions);


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

    private void writeList<T>(StreamWriter writer,Dictionary<String,List<T>> list)
    {
        foreach (var item in list)
        {
            writer.WriteLine(item.Key + " : " + ListTostring<T>(item.Value));
        }
        writer.WriteLine();
    }

    private void writeListOnly<T>(StreamWriter writer, List<T> list)
    {
        foreach(var item in list)
        {
            writer.Write(item.ToString() + ";");
        }
        writer.WriteLine();
    }

    private string ListTostring<T>(List<T> list)
    {
        string r = "";
        list.ForEach(w => r += w.ToString() + ";");
        return r;
    }
    private void ExtractAnimationData(AnimationClip animationClip,Transform targetTransform)
    {
        posGlobal.Clear();
        posLocal.Clear();
        rotGlobal.Clear();
        rotLocal.Clear();

        Transform[] childs = targetTransform.GetComponentsInChildren<Transform>(true);
        for(int i = 0; i < childs.Length; i++)
        {
            ExtractAnimPerTransform(animationClip, childs[i]);
        }
        
    }



    void ExtractAnimPerTransform(AnimationClip animationClip, Transform targetTransform)
    {
        // Get the number of frames in the animation clip
        int frameCount = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);

        //to store the joint rotations and positions

        List<Quaternion> rotG = new List<Quaternion>();
        List<Quaternion> rotL = new List<Quaternion>();
        List<Vector3> posG = new List<Vector3>();    
        List<Vector3> posL = new List<Vector3>();    

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
            posL.Add(localPosition);
            posG.Add(globalPosition);
            //Rot
            rotL.Add(localRotation);
            rotG.Add(globalRotation);
            

            // Reset the position and rotation of the target object to their original values
            targetTransform.position = position;
            targetTransform.rotation = rotation;
        }
        //pos
        posGlobal.Add(targetTransform.name, posG);
        posLocal.Add(targetTransform.name, posL);

        //rot
        rotGlobal.Add(targetTransform.name, rotG);
        rotLocal.Add(targetTransform.name, rotL);
    }
}


