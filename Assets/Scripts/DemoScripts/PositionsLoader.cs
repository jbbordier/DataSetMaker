using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using static Unity.Barracuda.TextureAsTensorData;
using UnityEngine.Playables;

public class PositionsLoader : MonoBehaviour
{
    public string path;
    public Transform origin;
    public Transform GT;
    public Transform to;

    private bool play = false;
    private int targetFrame = 0;
    private int current_frame = 0;
    public Dictionary<int, List<Vector3>> positionsTo = new Dictionary<int, List<Vector3>>();
    public Dictionary<int, List<Vector3>> positionsOrigin = new Dictionary<int, List<Vector3>>();
    public Dictionary<int, List<Vector3>> positionsGT = new Dictionary<int, List<Vector3>>();

    [ContextMenu("Load")]
    public void Load()
    {
        positionsTo = new Dictionary<int, List<Vector3>>();
        positionsOrigin = new Dictionary<int, List<Vector3>>();
        positionsGT = new Dictionary<int, List<Vector3>>();
        ExtractPos(path + "/AnimGTPos.txt", positionsGT);
        ExtractPos(path + "/AnimRefPos.txt", positionsOrigin);
        ExtractPos(path + "/AnimPos.txt", positionsTo);
        play = true;

    }

    private void ExtractPos(String path, Dictionary<int, List<Vector3>> pos)
    {
        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                Dictionary<EditorCurveBinding, AnimationCurve> bindings = new Dictionary<EditorCurveBinding, AnimationCurve>();
                string lines = reader.ReadToEnd();
                string[] line = lines.Split('\n');

                string[] newArray = new string[line.Length];
                Array.Copy(line, 0, newArray, 0, newArray.Length);
                //here is to handle the rest of the bones
                for (int a = 0; a < newArray.Length - 1; a++)
                {

                    if (newArray[a].Length > 1)
                    {
                        List<Vector3> list = new List<Vector3>();
                        string[] splittedLine = newArray[a].Split(';');
                        targetFrame = splittedLine.Length;
                        for (int j = 0; j < splittedLine.Length; j++)
                        {
                            try
                            {

                                string[] values = splittedLine[j].Split("|");
                                if (values.Length > 1)
                                {


                                    float x = float.Parse(values[0].Replace("(", "").Replace(".", ","));
                                    float y = float.Parse(values[1].Replace(".", ","));
                                    float z = float.Parse(values[2].Replace(")", "").Replace(".", ","));
                                    list.Add(new Vector3(x, y, z));

                                }

                            }
                            catch (Exception e)
                            {
                                Debug.Log(splittedLine + e.Message);
                            }

                        }
                        pos.Add(a, list);
                    }
                }
            }
        }

        catch { Debug.Log("Error on reading the file"); }

    }



    private void FixedUpdate()
    {
        if (play)
        {
            if (current_frame >= targetFrame-1)
            {
                current_frame = 0;
            }
            else
            {
                PlayAnim(GT, positionsGT);
                PlayAnim(to, positionsTo);
                PlayAnim(origin, positionsOrigin);
                current_frame++;
            }
        }

    }

    private void PlayAnim(Transform tr, Dictionary<int, List<Vector3>> pos)
    {
        int i = 0;
        Transform[] transforms = tr.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            if (pos.ContainsKey(i))
            {
                t.position = pos[i][current_frame];
            }
            i++;
        }
    }
}
