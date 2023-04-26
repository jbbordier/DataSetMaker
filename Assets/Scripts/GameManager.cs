using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject to;
    public AnimationList animList;
    public List<Transform> transformsFbx = new List<Transform>();//0 is from 1 is to
    public string folder;


    AnimationClip currentToClip;
    //each clip done for a specific from clip
    private List<AnimationClip> clipsDoneTo = new List<AnimationClip>();

    // animationDatas
    private Dictionary<string, List<Vector3>> posLocal = new Dictionary<string, List<Vector3>>();
    private Dictionary<string, List<Vector3>> posGlobal = new Dictionary<string, List<Vector3>>();
    private Dictionary<string, List<Quaternion>> rotLocal = new Dictionary<string, List<Quaternion>>();
    private Dictionary<string, List<Quaternion>> rotGlobal = new Dictionary<string, List<Quaternion>>();
    private int rotationNumber;
    private Quaternion origin = Quaternion.identity;
    private void Awake()
    {
        if (folder != "")
            transform.GetComponent<Recorder>().folder = folder;
        else
        {
            DirectoryInfo folder = Directory.CreateDirectory("Assets/Results/" + System.DateTime.Now.ToString("ddMMyyyy") + "_" + System.DateTime.Now.ToString("HH_m"));
            transform.GetComponent<Recorder>().folder = folder.FullName;
        }
        rotationNumber = 3;
        origin = to.transform.rotation;
        prepareAnimationClips();
        transform.GetComponent<Recorder>().toTransfrom = transformsFbx[1];
    }


    void prepareAnimationClips()
    {
        animList.clips.ForEach(x => clipsDoneTo.Add(x));
        moveToNextClip();
    }

    public void moveToNextClip()
    {
        if (rotationNumber >= 3)
        {
            if (currentToClip != null)
                clipsDoneTo.Remove(currentToClip);
            if (clipsDoneTo.Count > 0)
            {
                to.transform.rotation = origin;
                currentToClip = clipsDoneTo[0];
                transform.GetComponent<Recorder>().to = currentToClip;
                currentToClip.legacy = true;
                to.GetComponent<Animation>().AddClip(currentToClip, currentToClip.name);
                to.GetComponent<Animation>().Play(currentToClip.name);
            }
            rotationNumber = 0;
        }
        else
        {
            
            float random = UnityEngine.Random.Range(0f, 360f);
            to.transform.Rotate(Vector3.up, random);
            rotationNumber++;
            to.GetComponent<Animation>().Play(currentToClip.name);
        }
    }

    public void pauseAnimations()
    {
        to.GetComponent<Animation>().Stop();
    }

    [ContextMenu("Rewrite")]
    public void RewriteAnim()
    {
        List<string> olds = new List<string>();

        string all = "";
        using (StreamReader reader = new StreamReader("Assets/Results/" + folder + "/datasAnim.txt"))
        {
            string content = reader.ReadToEnd();
            all = content;
            string[] splittedContent = content.Split("Animations :");
            for (int i = 1; i < splittedContent.Length; i++)
            {
                string[] split = splittedContent[i].Split("To ");
                olds.Add(split[1]);
                olds.Add(split[2]);
                olds.Add(split[3]);
                olds.Add(split[4]);
            }
        }
        using (StreamWriter writer = new StreamWriter("Assets/Results/" + folder + " /datasAnim.txt"))
        {
            int i = 0;
            foreach (var item in animList.clips)
            {
                ExtractAnimationData(item, to.transform);
                string localRot = "\nTo animation Local Rotations : ";
                string GlobalRot = "\n\nTo animation Global Rotations : ";
                string localPos = "\n\nTo animation Local Positions : ";
                string GlobalPos = "\n\nTo animation Global Positions : ";

                foreach (var localR in rotLocal)
                {
                    if (localR.Key != "To" && localR.Key != "Eyelashes" && localR.Key != "Body" && localR.Key != "Bottoms" && localR.Key != "Eyes" && localR.Key != "Hair" && localR.Key != "Shoes" && localR.Key != "Tops")
                    {
                        localRot += '\n';
                        localRot += localR.Key + " : " + ListTostring<Quaternion>(localR.Value);
                    }

                }
                foreach (var globalR in rotGlobal)
                {
                    if (globalR.Key != "To" && globalR.Key != "Eyelashes" && globalR.Key != "Body" && globalR.Key != "Bottoms" && globalR.Key != "Eyes" && globalR.Key != "Hair" && globalR.Key != "Shoes" && globalR.Key != "Tops")
                    {
                        GlobalRot += '\n';
                        GlobalRot += globalR.Key + " : " + ListTostring<Quaternion>(globalR.Value);
                    }
                }
                foreach (var localP in posLocal)
                {
                    if (localP.Key != "To" && localP.Key != "Eyelashes" && localP.Key != "Body" && localP.Key != "Bottoms" && localP.Key != "Eyes" && localP.Key != "Hair" && localP.Key != "Shoes" && localP.Key != "Tops")
                    {
                        localPos += '\n';
                        localPos += localP.Key + " : " + ListTostring<Vector3>(localP.Value);
                    }
                }
                foreach (var globalP in posGlobal)
                {
                    if (globalP.Key != "To" && globalP.Key != "Eyelashes" && globalP.Key != "Body" && globalP.Key != "Bottoms" && globalP.Key != "Eyes" && globalP.Key != "Hair" && globalP.Key != "Shoes" && globalP.Key != "Tops")
                    {
                        GlobalPos += '\n';
                        GlobalPos += globalP.Key + " : " + ListTostring<Vector3>(globalP.Value);
                    }
                }

                all = all.Replace(olds[0 + (i)], localRot);
                all = all.Replace(olds[1 + (i)], GlobalRot);
                all = all.Replace(olds[2 + (i)], localPos);
                all = all.Replace(olds[3 + (i)], GlobalPos);
                i += 4;
            }
            all = all.Replace(";To ", "");
            writer.Write(all);

        }
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
        // Get the number of frames in the animation clip
        int frameCount = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);
        Transform[] childs = targetTransform.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < frameCount; i++)
        {
            float time = i / animationClip.frameRate;
            // Sample the animation clip at the current time
            animationClip.SampleAnimation(targetTransform.gameObject, time);
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
        Vector3 globalPosition = targetTransform.position;

        //Rot
        Quaternion localRotation = targetTransform.localRotation;
        Quaternion globalRotation = targetTransform.rotation;
        //to store the joint rotations and positions
        if (!rotGlobal.TryGetValue(targetTransform.name, out rotG))
        {
            rotG = new List<Quaternion>();
            rotL = new List<Quaternion>();
            posG = new List<Vector3>();
            posL = new List<Vector3>();
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
