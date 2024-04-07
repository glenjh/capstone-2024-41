using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMove : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 lastCamPos;
    [SerializeField] Vector2 moveSpeed;

    void Start()
    {
        camTransform = Camera.main.transform;
        lastCamPos = camTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = camTransform.position - lastCamPos;
        transform.position += new Vector3(deltaMovement.x * moveSpeed.x, deltaMovement.y * moveSpeed.y);
        lastCamPos = camTransform.position;
    }
}