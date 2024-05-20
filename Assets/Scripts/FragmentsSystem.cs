using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentsSystem : MonoBehaviour
{
    [SerializeField] Fragment[] fragmentprefab;
    [SerializeField] bool isRotate = true;
    [SerializeField] float rotateSpeed = 0;
    [SerializeField] float nomalSpeed = 1;
    [SerializeField] float lifeTime = 0;
    
    public void InstantiateFragment(Transform impactPos, Transform scale, float power)
    {
        Vector2 direction = (transform.position - impactPos.position).normalized;
        direction = new Vector2(direction.x, direction.y + 0.5f);
        
        foreach( var fragment in fragmentprefab)
        {
            Fragment fragmentObj = Instantiate(fragment);
            
            //Vector2 randomDirection = Quaternion.Euler(0, 0, UnityEngine.Random.Range(60, 120)) * direction;
            fragmentObj.GetComponent<Fragment>().Init(transform, scale, direction, nomalSpeed*power, lifeTime, isRotate, rotateSpeed);
        }
    }
}
