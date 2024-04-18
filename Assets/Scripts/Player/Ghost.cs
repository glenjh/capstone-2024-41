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
                GameObject currGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currSprite = GetComponent<SpriteRenderer>().sprite;
                currGhost.transform.localScale = this.transform.localScale;
                currGhost.GetComponent<SpriteRenderer>().sprite = currSprite;
                ghostDelay = ghostDelaySeconds;
                Destroy(currGhost, 1f);
            }
        }
    }
}
