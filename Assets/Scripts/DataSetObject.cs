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
    public Transform VRRig;


    // animationDatas
    private Dictionary<String, List<Vector3>> posLocal = new Dictionary<string, List<Vector3>>();
    private Dictionary<String, List<Vector3>> posGlobal = new Dictionary<string, List<Vector3>>();
    private Dictionary<String, List<Quaternion>> rotLocal = new Dictionary<string, List<Quaternion>>();
    private Dictionary<String, List<Quaternion>> rotGlobal = new Dictionary<string, List<Quaternion>>();
    private List<float> distanceL = new List<float>();
    private List<float> distanceR = new List<float>();




    public DataSetObject(AnimationClip to, Transform trTo)
    {
        clipTo = to;
        transformTo = trTo;
    }
    public void ExportToJson(string folder, bool append = true)
    {
        using (StreamWriter writer = new StreamWriter(folder + "/datasAnim.txt", append))
        {
            //animations names from --> to
            writer.WriteLine("Animations :");
            if (clipTo != null)
                writer.WriteLine(clipTo.name);
            else
                writer.WriteLine("simulating");


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

            if (transformTo != null)
            {

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

                writer.WriteLine("To animation LeftFoot Distance : ");
                writeListOnly<float>(writer, distanceL);

                writer.WriteLine("To animation RightFoot Distance : ");
                writeListOnly<float>(writer, distanceR);
            }
        }

    }

    private void writeList<T>(StreamWriter writer, Dictionary<String, List<T>> list)
    {
        foreach (var item in list)
        {
            writer.WriteLine(item.Key + " : " + ListTostring<T>(item.Value));
        }
        writer.WriteLine();
    }

    private void writeListOnly<T>(StreamWriter writer, List<T> list)
    {
        foreach (var item in list)
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
    public void ExtractAnimationData(AnimationClip animationClip, Transform targetTransform)
    {
        posGlobal.Clear();
        posLocal.Clear();
        rotGlobal.Clear();
        rotLocal.Clear();
        distanceL.Clear();
        distanceR.Clear();
        // Get the number of frames in the animation clip
        int frameCount = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);
        Transform[] childs = targetTransform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < frameCount; i++)
        {
            float time = i / animationClip.frameRate;
            // Sample the animation clip at the current time
            animationClip.SampleAnimation(targetTransform.parent.gameObject, time);
            ExtractAnimPerTransform(targetTransform.parent);
            GameObject foottR = GameObject.Find("mixamorig:RightFoot");
            GameObject foottL = GameObject.Find("mixamorig:RightFoot");

            float dist = 0f;

            if(Physics.Raycast(foottR.transform.position,Vector3.down,out RaycastHit hitR))
            {
                dist = hitR.distance;
                distanceL.Add(dist);
            }
            if (Physics.Raycast(foottL.transform.position, Vector3.down, out RaycastHit hitL))
            {
                dist = hitL.distance;
                distanceR.Add(dist);
            }

            for (int j = 0; j < childs.Length; j++)
            {
                ExtractAnimPerTransform(childs[j]);
            }
        }

    }



    void ExtractAnimPerTransform(Transform targetTransform)
    {

        List<Quaternion> rotG = new List<Quaternion>();
        List<Quaternion> rotL = new List<Quaternion>();
        List<Vector3> posG = new List<Vector3>();
        List<Vector3> posL = new List<Vector3>();

        // Get the new position and rotation of the target object after the animation clip has been sampled

        //Pos
        Vector3 localPosition = targetTransform.localPosition;
        var results = VRRig.worldToLocalMatrix * targetTransform.localToWorldMatrix;
        Vector3 globalPosition = results.GetPosition();

        //Rot
        Quaternion localRotation = targetTransform.localRotation;
        Quaternion globalRotation = results.rotation;
        //to store the joint rotations and positions
        if (!rotGlobal.TryGetValue(targetTransform.name, out rotG))
        {
            rotG = new List<Quaternion>();
            rotG.Add(globalRotation);
            rotL.Add(localRotation);

            posG.Add(globalPosition);
            posL.Add(localPosition);
            //pos
            posGlobal.Add(targetTransform.name, posG);
            posLocal.Add(targetTransform.name, posL);

            //rot
            rotGlobal.Add(targetTransform.name, rotG);
            rotLocal.Add(targetTransform.name, rotL);
        }
        else
        {
            rotGlobal.TryGetValue(targetTransform.name, out rotG);
            rotLocal.TryGetValue(targetTransform.name, out rotL);
            posGlobal.TryGetValue(targetTransform.name, out posG);
            posLocal.TryGetValue(targetTransform.name, out posL);

            rotG.Add(globalRotation);
            rotL.Add(localRotation);

            posG.Add(globalPosition);
            posL.Add(localPosition);

        }

    }
}


