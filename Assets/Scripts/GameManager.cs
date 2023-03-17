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

    private void Awake()
    {
        if (folder != "")
            transform.GetComponent<Recorder>().folder = folder;
        else
        {
            DirectoryInfo folder = Directory.CreateDirectory("Assets/Results/" + System.DateTime.Now.ToString("ddMMyyyy") + "_" + System.DateTime.Now.ToString("HH_m"));
            transform.GetComponent<Recorder>().folder = folder.FullName;
        }
        prepareAnimationClips();
        transform.GetComponent<Recorder>().toTransfrom = transformsFbx[1];
    }


    void prepareAnimationClips()
    {
        clipsDoneTo = animList.clips;
        moveToNextClip();
    }

    public void moveToNextClip()
    {
        if (currentToClip != null)
            clipsDoneTo.Remove(currentToClip);
        if (clipsDoneTo.Count > 0)
        {
            currentToClip = clipsDoneTo[0];
            transform.GetComponent<Recorder>().to = currentToClip;
            currentToClip.legacy = true;
            to.GetComponent<Animation>().AddClip(currentToClip, currentToClip.name);
            to.GetComponent<Animation>().Play(currentToClip.name);
        }

    }

    public void pauseAnimations()
    {
        to.GetComponent<Animation>().Stop();
    }
}
