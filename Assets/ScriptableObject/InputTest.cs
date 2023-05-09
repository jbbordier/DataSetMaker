using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputTest", menuName = "ScriptableObjects/InputTest", order = 1)]
public class InputTest : ScriptableObject
{
    public List<Quaternion> LeftRot;
    public List<Quaternion> RightRot;
    public List<Vector3> LeftPos;
    public List<Vector3> RightPos;
}
