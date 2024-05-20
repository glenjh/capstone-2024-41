using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject ghost;
    public bool makeGhost = false;
    
    void Start()
    {
        ghostDelay = ghostDelaySeconds;
    }
    
    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelay > 0)
            {
                ghostDelay -= Time.deltaTime;
            }
            else
            {
                var currGhost = PoolManager.Instance.GetFromPool<Effects>("Ghost");
                currGhost.transform.position = this.transform.position;
                currGhost.transform.localScale = this.transform.localScale;
                
                Sprite currSprite = GetComponent<SpriteRenderer>().sprite;
                currGhost.GetComponent<SpriteRenderer>().sprite = currSprite;
                ghostDelay = ghostDelaySeconds;
            }
        }
    }
}
