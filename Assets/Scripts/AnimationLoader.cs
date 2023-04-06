using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AnimationLoader : MonoBehaviour
{
    public string path;
    public AnimationClip clip;
    public Transform fbx;
    public float framerate = 60;

    public Transform root;
    private void Start()
    {

        Animation anim = fbx.parent.GetComponent<Animation>();
        if (anim.GetClipCount() > 0)
            anim.RemoveClip(clip);
        clip.ClearCurves();
        ApplyToCharacter();
        clip.legacy = true;
        //clip.frameRate = framerate;
        anim.AddClip(clip, "loadedAnim");
        anim.Play("loadedAnim");

    }
    private void ApplyToCharacter()
    {
        Transform[] tr = fbx.GetComponentsInChildren<Transform>();
        using (StreamReader reader = new StreamReader(path + "/Anim.txt"))
        {
            Dictionary<EditorCurveBinding, AnimationCurve> bindings = new Dictionary<EditorCurveBinding, AnimationCurve>();
            string lines = reader.ReadToEnd();
            string[] line = lines.Split('\n');
            string[] newArray = new string[line.Length - 1];
            Array.Copy(line, 1, newArray, 0, newArray.Length);
            Quaternion previous = Quaternion.identity;
            for (int a = 0; a < newArray.Length; a++)
            {

                if (newArray[a].Length > 1)
                {
                    AnimationCurve cX = new AnimationCurve();
                    AnimationCurve cY = new AnimationCurve();
                    AnimationCurve cZ = new AnimationCurve();
                    AnimationCurve cW = new AnimationCurve();
                    string[] splittedLine = newArray[a].Split(';');
                    Debug.Log(splittedLine.Length);
                    for (int j = 0; j < splittedLine.Length; j++)
                    {
                        try
                        {

                            string[] values = splittedLine[j].Split("|");
                            if (values.Length > 1)
                            {


                                float x = float.Parse(values[0].Replace("(", "").Replace(".", ","));
                                float y = float.Parse(values[1].Replace(".", ","));
                                float z = float.Parse(values[2].Replace(".", ","));
                                float w = float.Parse(values[3].Replace(")", "").Replace(".", ","));
                                Quaternion q;
                                if(previous == Quaternion.identity)
                                {
                                    q = new Quaternion(x, y, z, w);
                                    previous = q;
                                }
                                else
                                {
                                     q = new Quaternion(x, y, z, w) * Quaternion.Inverse(previous);
                                    previous = new Quaternion(x, y, z, w);
                                }
                                
                                cX.AddKey(new Keyframe((j + 1) / framerate, q.x));
                                cY.AddKey(new Keyframe((j + 1) / framerate, q.y));
                                cZ.AddKey(new Keyframe((j + 1) / framerate, q.z));
                                cW.AddKey(new Keyframe((j + 1) / framerate, q.w));
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.Log(splittedLine + e.Message);
                        }

                    }
                    AddCurveBindings(cX, cY, cZ, cW, tr[a]);
                }



            }


        }
    }

    public void AddCurveBindings(AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, AnimationCurve curveW, Transform go)
    {
        EditorCurveBinding binding = new EditorCurveBinding();
        binding.type = typeof(Transform);
        binding.path = AnimationUtility.CalculateTransformPath(go, root);
        binding.propertyName = "m_LocalRotation.x";
        AnimationUtility.SetEditorCurve(clip, binding, curveX);
        binding.propertyName = "m_LocalRotation.y";
        AnimationUtility.SetEditorCurve(clip, binding, curveY);
        binding.propertyName = "m_LocalRotation.z";
        AnimationUtility.SetEditorCurve(clip, binding, curveZ);
        binding.propertyName = "m_LocalRotation.w";
        AnimationUtility.SetEditorCurve(clip, binding, curveW);

    }

}
