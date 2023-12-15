using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MakeClipsLegacy : MonoBehaviour
{


    private AnimationList clipsList;
    // Start is called before the first frame update
    void Start()
    {

        clipsList = GetComponent<GameManager>().animList;
        foreach (var item in clipsList.clips)
        {
            item.legacy = true;
            print(item.name+" Framerate :" + item.frameRate + " and length :" + item.length + "so :" + Mathf.RoundToInt(item.length * item.frameRate));
            item.wrapMode = WrapMode.Loop;
            float time = Random.Range(1.5f, 5.5f);
            changeLength(item,time);
         
            
        }
    }


    public void changeLength(AnimationClip animClip, float time)
    {
        float newFrameRate = time / animClip.length;
        // Scale keyframe positions.
        int max = 0;
        EditorCurveBinding [] curves = AnimationUtility.GetCurveBindings(animClip);
        foreach (EditorCurveBinding curveBinding in curves)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(animClip, curveBinding);
            Keyframe[] keyframes = curve.keys;
            max = Mathf.Max(max, keyframes.Length);
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i].time *= newFrameRate;
            }
            curve.keys = keyframes;
            AnimationUtility.SetEditorCurve(animClip, curveBinding, curve);
        }

        print(animClip.name + " has " + max + " keyframes and time has been set to" + time);
    }


}
