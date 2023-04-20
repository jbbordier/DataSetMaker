using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    public string path;

    public void writeFile(List<Quaternion> lQ, List<Quaternion> rQ, List<Vector3> lP, List<Vector3> rP)
    {
        DataSetObject dataObj = new DataSetObject(null, null);
        dataObj.LeftHandQuaternions = lQ;
        dataObj.RightHandQuaternions = rQ;
        dataObj.LeftHandPositions = lP;
        dataObj.RightHandPositions = rP;
        dataObj.ExportToJson(path,false);
    }

}
