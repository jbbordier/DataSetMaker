using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointPositionPrinter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Global: " + transform.position);
        Debug.Log("Locale: " + transform.localPosition);
    }
}
