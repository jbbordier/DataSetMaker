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
    public Transform leftHand;
    public Transform rightHand;
    public Transform root;
    public AnimationClip left;
    public AnimationClip right;



    public void Load()
    {

        ClearAnims();

        ApplyToCharacter();

        //clip.frameRate = framerate;
        Animation anim = fbx.parent.GetComponent<Animation>();
        anim.AddClip(clip, "loadedAnim");
        anim.Play("loadedAnim");
        anim = leftHand.GetComponent<Animation>();
        anim.AddClip(left, "loadedAnim");
        anim.Play("loadedAnim");
        anim = rightHand.GetComponent<Animation>();
        anim.AddClip(right, "loadedAnim");
        anim.Play("loadedAnim");
    }

    void ClearAnims()
    {
        Animation anim = fbx.parent.GetComponent<Animation>();
        if (anim.GetClipCount() > 0)
            anim.RemoveClip(clip);
        clip.ClearCurves();
        anim = leftHand.GetComponent<Animation>();
        if (anim.GetClipCount() > 0)
            anim.RemoveClip(left);
        anim = rightHand.GetComponent<Animation>();
        if (anim.GetClipCount() > 0)
            anim.RemoveClip(right);

        clip.legacy = true;
        left.legacy = true;
        right.legacy = true;

        clip.ClearCurves();
        right.ClearCurves();
        left.ClearCurves();
    }
    private void ApplyToCharacter()
    {
        Transform[] tr = fbx.GetComponentsInChildren<Transform>();
        using (StreamReader reader = new StreamReader(path + "/Anim.txt"))
        {
            Dictionary<EditorCurveBinding, AnimationCurve> bindings = new Dictionary<EditorCurveBinding, AnimationCurve>();
            string lines = reader.ReadToEnd();
            string[] line = lines.Split('\n');
            string[] newArray = new string[line.Length - 5];
            string[] hands = new string[4];
            Array.Copy(line, line.Length - 4, hands, 0, hands.Length);
            Array.Copy(line, 1, newArray, 0, newArray.Length);
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
                                cX.AddKey(new Keyframe((j + 1) / framerate, x));
                                cY.AddKey(new Keyframe((j + 1) / framerate, y));
                                cZ.AddKey(new Keyframe((j + 1) / framerate, z));
                                cW.AddKey(new Keyframe((j + 1) / framerate, w));
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.Log(splittedLine + e.Message);
                        }

                    }
                    AddCurveBindings(clip, tr[a], cX, cY, cZ, cW);
                }



            }
            for (int a = 1; a < hands.Length; a = a + 2)
            {
                AnimationCurve cX = new AnimationCurve();
                AnimationCurve cY = new AnimationCurve();
                AnimationCurve cZ = new AnimationCurve();
                AnimationCurve cW = new AnimationCurve();
                AnimationCurve cxPos = new AnimationCurve();
                AnimationCurve cyPos = new AnimationCurve();
                AnimationCurve czPos = new AnimationCurve();
                string[] splittedLine = hands[a].Split(';');
                for (int i = 0; i < splittedLine.Length; i++)
                {
                    string[] posRot = splittedLine[i].Split('|');
                    string[] pos = posRot[0].Split(',');
                    if (pos.Length > 1)
                    {
                        float posx = float.Parse(pos[0].Replace("(", "").Replace(".", ","));
                        float posy = float.Parse(pos[1].Replace(".", ","));
                        float posz = float.Parse(pos[2].Replace(")", "").Replace(".", ","));
                        string[] rot = posRot[1].Split(',');
                        float x = float.Parse(rot[0].Replace("(", "").Replace(".", ","));
                        float y = float.Parse(rot[1].Replace(".", ","));
                        float z = float.Parse(rot[2].Replace(".", ","));
                        float w = float.Parse(rot[3].Replace(")", "").Replace(".", ","));
                        cX.AddKey(new Keyframe((i + 1) / framerate, x));
                        cY.AddKey(new Keyframe((i + 1) / framerate, y));
                        cZ.AddKey(new Keyframe((i + 1) / framerate, z));
                        cW.AddKey(new Keyframe((i + 1) / framerate, w));
                        cxPos.AddKey(new Keyframe((i + 1) / framerate, posx));
                        cyPos.AddKey(new Keyframe((i + 1) / framerate, posy));
                        czPos.AddKey(new Keyframe((i + 1) / framerate, posz));
                    }

                }
                if (a == 1)
                {
                    AddCurveBindings(left, leftHand, cX, cY, cZ, cW, cxPos, cyPos, czPos);
                }
                else
                {
                    AddCurveBindings(right, rightHand, cX, cY, cZ, cW, cxPos, cyPos, czPos);
                }

            }

        }
    }

    public void AddCurveBindings(AnimationClip clip, Transform go, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, AnimationCurve curveW, AnimationCurve curveXpos = null, AnimationCurve curveYpos = null, AnimationCurve curveZpos = null)
    {
        EditorCurveBinding binding = new EditorCurveBinding();
        binding.type = typeof(Transform);
        if (curveXpos == null)
            binding.path = AnimationUtility.CalculateTransformPath(go, root);


        binding.propertyName = "m_LocalRotation.x";
        AnimationUtility.SetEditorCurve(clip, binding, curveX);
        binding.propertyName = "m_LocalRotation.y";
        AnimationUtility.SetEditorCurve(clip, binding, curveY);
        binding.propertyName = "m_LocalRotation.z";
        AnimationUtility.SetEditorCurve(clip, binding, curveZ);
        binding.propertyName = "m_LocalRotation.w";
        AnimationUtility.SetEditorCurve(clip, binding, curveW);
        if (curveXpos != null)
        {
            binding.propertyName = "m_LocalPosition.x";
            AnimationUtility.SetEditorCurve(clip, binding, curveXpos);
            binding.propertyName = "m_LocalPosition.y";
            AnimationUtility.SetEditorCurve(clip, binding, curveYpos);
            binding.propertyName = "m_LocalPosition.z";
            AnimationUtility.SetEditorCurve(clip, binding, curveZpos);
        }

    }

}
