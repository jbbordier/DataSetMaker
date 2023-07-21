using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fbxFileNameRemover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform[] go = transform.GetComponentsInChildren<Transform>();
        foreach (Transform g in go)
        {
            if (g.gameObject.name.Contains("Model:"))
            {
                g.gameObject.name = g.gameObject.name.Replace("Model:", "");
            }
        }
    }

}
