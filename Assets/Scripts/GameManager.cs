using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject from;
    public GameObject to;
    public AnimationList animList;
    public List<Transform> transformsFbx = new List<Transform>();//0 is from 1 is to
    public string folder;

    //clips to do as from
    private List<AnimationClip> clipsFrom = new List<AnimationClip>();
    AnimationClip currentFromClip;
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
        transform.GetComponent<Recorder>().fromTransfrom = transformsFbx[0];
        transform.GetComponent<Recorder>().toTransfrom = transformsFbx[1];
    }


    void prepareAnimationClips()
    {
        clipsFrom.Clear();
        clipsDoneTo.Clear();
        foreach (var clip in animList.clips)
        {
            clipsFrom.Add(clip);
        }
        moveFromNextClip();
        moveToNextClip();
    }

    public void ChangeClip()
    {
        if(clipsDoneTo.Count == animList.clips.Count - 1)
        {
            moveFromNextClip();
            moveToNextClip();
        }
        else
        {
            moveToNextClip();
            from.GetComponent<Animation>().Play(currentFromClip.name);
        }
    }

    public void moveFromNextClip()
    { //this method to change from clip, reset all to clips
        if (currentFromClip != null)
        {
            from.GetComponent<Animation>().RemoveClip(currentFromClip);
            clipsFrom.Remove(currentFromClip);
        }
        if (clipsFrom.Count > 0)
        {
            currentFromClip = clipsFrom[0];
            clipsDoneTo.Clear();
            gameObject.GetComponent<Recorder>().from = currentFromClip;
            currentFromClip.legacy = true;
            from.GetComponent<Animation>().AddClip(currentFromClip, currentFromClip.name);
            from.GetComponent<Animation>().Play(currentFromClip.name);
        }
    }

    public void moveToNextClip()
    {
        bool founded = false;
        if (currentToClip != null)
            to.GetComponent<Animation>().RemoveClip(currentToClip);
        animList.clips.ForEach(x =>
        {
            //check if x has not already be done for this from clip and if its not the same
            if (!clipsDoneTo.Contains(x) && x != currentFromClip &&!founded)
            {
                currentToClip = x;
                clipsDoneTo.Add(x);
                founded = true;
            }
        });
        gameObject.GetComponent<Recorder>().to = currentToClip;
        currentToClip.legacy = true;
        to.GetComponent<Animation>().AddClip(currentToClip, currentToClip.name);
        to.GetComponent<Animation>().Play(currentToClip.name);
    }

    public void pauseAnimations()
    {
        from.GetComponent<Animation>().Stop();
        to.GetComponent<Animation>().Stop();
    }
}
