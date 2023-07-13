using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandsDataLoader : MonoBehaviour
{


    public string pathAnimTraining;
    public string pathAnimTesting = "Assets/Results/Tester/datasAnim.txt";
    public string animName;

    public List<Quaternion> lRot;
    public List<Quaternion> rRot;
    public List<Vector3> lPos;
    public List<Vector3> rPos;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    [ContextMenu("Load")]
    void LoadAnim()
    {

        List<Quaternion>  lRot = new List<Quaternion>();
       List<Quaternion>  rRot = new List<Quaternion>();
       List<Vector3>  lPos = new List<Vector3>();
       List<Vector3>  rPos = new List<Vector3>();

    }


    void getHandsPositionRotation(string path,string animName, List<Vector3> lposition, List<Vector3> rposition, List<Quaternion> lrotation, List<Quaternion> rrotation)
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
