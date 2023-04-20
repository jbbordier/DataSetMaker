using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandsDataLoader : MonoBehaviour
{

    public string path;
    public int animIndex;

    public List<Quaternion> lRot;
    public List<Quaternion> rRot;
    public List<Vector3> lPos;
    public List<Vector3> rPos;
    // Start is called before the first frame update
    void Start()
    {
        lRot = new List<Quaternion>();
        rRot = new List<Quaternion>();
        lPos = new List<Vector3>();
        rPos = new List<Vector3>();
        if (path != "")
            LoadAnim();
    }

    void LoadAnim()
    {
        using(StreamReader reader = new StreamReader(path))
        {
            string content = reader.ReadToEnd();
            string[] anim = content.Split("Animations :");
            string[] frame = anim[animIndex].Split("\n");
            string[] leftQuat = frame[2+1].Split(';');
            string[] rightQuat = frame[4+1].Split(';');
            string[] leftPos = frame[6 + 1].Split(';');
            string[] rightPos = frame[8 + 1].Split(';');

            for(int i =0; i < leftQuat.Length-1; i++)
            {
                string[] quat = leftQuat[i].Split(',');
                lRot.Add(new Quaternion(float.Parse(quat[0].Replace("(","").Replace('.',',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")","").Replace('.', ','))));
                quat = rightQuat[i].Split(',');
                rRot.Add(new Quaternion(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace('.', ',')), float.Parse(quat[3].Replace(")", "").Replace('.', ','))));
                quat = leftPos[i].Split(',');
                lPos.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
                quat = rightPos[i].Split(',');
                rPos.Add(new Vector3(float.Parse(quat[0].Replace("(", "").Replace('.', ',')), float.Parse(quat[1].Replace('.', ',')), float.Parse(quat[2].Replace(")", "").Replace('.', ','))));
            }

        }
    }



}
