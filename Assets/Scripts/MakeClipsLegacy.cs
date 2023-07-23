using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeClipsLegacy : MonoBehaviour
{

    public List<AnimationClip> clips = new List<AnimationClip>(); 
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in clips)
        {
            item.legacy = true;
            print(item.name+" Framerate :" + item.frameRate + " and length :" + item.length + "so :" + Mathf.RoundToInt(item.length * item.frameRate));
            item.wrapMode = WrapMode.Loop;
        }
    }


}
