using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour {
    public Action Interaction;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("NarrationObject"))
        {
            //NarrationManager.Instance.ShowNarration(other.gameObject.GetComponent<TalkTest>().GetNarration());
            other.gameObject.GetComponent<TalkTest>().GetNarration();
        }
    }
    
    public void AddInteraction(Action interaction)
    {
        Interaction += interaction;
    }

    private void Update()
    {
        if(Interaction != null)
            Interaction();
    }
}