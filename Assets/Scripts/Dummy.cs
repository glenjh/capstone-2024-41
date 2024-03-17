using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public int HP = 3;

    public void TakeDamege()
    {
        if (HP > 0)
        {
            HP -= 1;
        }
        else
        {
            Debug.Log("Dummy Dead");
            GameObject.Destroy(gameObject);
        }
    }
}
