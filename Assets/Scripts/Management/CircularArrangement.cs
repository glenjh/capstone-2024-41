using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircularArrangement : MonoBehaviour
{
    public float radius = 1f;
    public float amount = 0.5f;
    void Start()
    {
        int numOfChild = transform.childCount;

        for (int i = 0; i < numOfChild; i++)
        {
            float angle = Mathf.PI * amount -  i * (Mathf.PI * 2.0f) / numOfChild;
            GameObject child = transform.GetChild(i).gameObject;

            child.transform.position = transform.position + (new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0)) * radius;
        }
    }
    
}
